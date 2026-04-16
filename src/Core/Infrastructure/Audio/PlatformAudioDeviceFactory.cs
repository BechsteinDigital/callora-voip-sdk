using System.Reflection;
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Ports.Audio;

namespace CalloraVoipSdk.Core.Infrastructure.Audio;

/// <summary>
/// Resolves the effective audio device for the current runtime environment.
/// Falls back to <see cref="SilenceAudioDevice"/> when no platform provider is available.
/// </summary>
internal static class PlatformAudioDeviceFactory
{
    /// <summary>
    /// Returns either the configured device or an auto-selected platform device.
    /// </summary>
    public static IAudioDevice Resolve(
        IAudioDevice configured,
        bool enableAutoSelection,
        ILogger logger,
        out bool ownsResolvedDevice)
    {
        ownsResolvedDevice = false;

        if (!enableAutoSelection || configured is not SilenceAudioDevice)
            return configured;

        if (TryCreatePlatformDevice(logger, out var platformAudio, out var reason))
        {
            ownsResolvedDevice = true;
            logger.LogInformation("Using platform audio device: {AudioDevice}", platformAudio.Name);
            return platformAudio;
        }

        logger.LogInformation("Using silence audio device ({Reason}).", reason);
        return configured;
    }

    /// <summary>
    /// Attempts to create a platform-specific audio device based on the current OS.
    /// </summary>
    private static bool TryCreatePlatformDevice(
        ILogger logger,
        out IAudioDevice device,
        out string reason)
    {
        if (OperatingSystem.IsWindows())
            return TryCreateFromAssembly(
                "CalloraVoipSdk.Audio.Windows",
                "CalloraVoipSdk.Audio.Windows.WindowsAudioDevice",
                logger,
                out device,
                out reason);

        if (OperatingSystem.IsLinux())
            return TryCreateFromAssembly(
                "CalloraVoipSdk.Audio.Linux",
                "CalloraVoipSdk.Audio.Linux.LinuxAudioDevice",
                logger,
                out device,
                out reason);

        device = SilenceAudioDevice.Instance;
        reason = "no platform audio provider for this operating system";
        return false;
    }

    /// <summary>
    /// Attempts to load an audio device type from an external assembly and instantiate it.
    /// </summary>
    private static bool TryCreateFromAssembly(
        string assemblyName,
        string typeName,
        ILogger logger,
        out IAudioDevice device,
        out string reason)
    {
        try
        {
            var assembly = FindLoadedAssembly(assemblyName) ?? Assembly.Load(assemblyName);
            var type = assembly.GetType(typeName, throwOnError: false);
            if (type is null)
            {
                device = SilenceAudioDevice.Instance;
                reason = $"type '{typeName}' not found";
                return false;
            }

            var ctor = type.GetConstructor(Type.EmptyTypes)
                ?? type.GetConstructors().FirstOrDefault(c =>
                    c.GetParameters().All(p => p.IsOptional));
            if (ctor is null || ctor.Invoke(ctor.GetParameters()
                    .Select(p => p.DefaultValue)
                    .ToArray()) is not IAudioDevice audioDevice)
            {
                device = SilenceAudioDevice.Instance;
                reason = $"type '{typeName}' is not an IAudioDevice";
                return false;
            }

            device = audioDevice;
            reason = "ok";
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to create platform audio device from {AssemblyName}/{TypeName}. Falling back to silence.",
                assemblyName,
                typeName);
            device = SilenceAudioDevice.Instance;
            reason = ex.GetBaseException().Message;
            return false;
        }
    }

    /// <summary>
    /// Finds an already loaded assembly by simple name.
    /// </summary>
    private static Assembly? FindLoadedAssembly(string assemblyName) =>
        AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(
                a.GetName().Name,
                assemblyName,
                StringComparison.Ordinal));
}
