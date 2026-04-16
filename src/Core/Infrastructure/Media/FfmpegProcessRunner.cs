using System.Diagnostics;
using System.Text;
using System.ComponentModel;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

/// <summary>
/// Runs ffmpeg processes for media transcode operations with structured error handling.
/// </summary>
internal static class FfmpegProcessRunner
{
    /// <summary>
    /// Executes one ffmpeg command and throws on non-zero exit status.
    /// </summary>
    public static async Task RunAsync(
        Action<ProcessStartInfo> configure,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        configure(psi);

        using var process = new Process { StartInfo = psi };

        try
        {
            if (!process.Start())
                throw new InvalidOperationException("Failed to start ffmpeg process.");
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            throw new InvalidOperationException(
                "Unable to start ffmpeg. Ensure ffmpeg is installed and available in PATH.",
                ex);
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = process.StandardError.ReadToEndAsync(ct);

        await process.WaitForExitAsync(ct).ConfigureAwait(false);

        var stdout = await stdoutTask.ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);

        if (process.ExitCode == 0)
            return;

        var sb = new StringBuilder();
        sb.Append("ffmpeg failed with exit code ").Append(process.ExitCode).Append('.');

        if (!string.IsNullOrWhiteSpace(stderr))
            sb.Append(" stderr: ").Append(stderr.Trim());
        else if (!string.IsNullOrWhiteSpace(stdout))
            sb.Append(" output: ").Append(stdout.Trim());

        throw new InvalidOperationException(sb.ToString());
    }

    /// <summary>
    /// Checks if ffmpeg is invokable in the current runtime environment.
    /// </summary>
    public static bool IsAvailable()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            psi.ArgumentList.Add("-version");

            using var process = Process.Start(psi);
            if (process is null)
                return false;

            process.WaitForExit(1000);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
