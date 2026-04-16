using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Audio;
using CalloraVoipSdk.Core.Domain.Calls;
using CalloraVoipSdk.Core.Domain.Events;

namespace CalloraVoipSdk.Core.Application.Convenience;

/// <summary>
/// Lifecycle wrapper for default call audio wiring.
/// </summary>
internal sealed class DefaultAudioCallAttachment : IDisposable
{
    private readonly ICall _call;
    private readonly IAudioDevice _audioDevice;
    private readonly IMediaReceiver _receiver;
    private readonly IMediaSender _sender;
    private readonly ILogger<DefaultAudioCallAttachment> _logger;
    private readonly Action<CallId, DefaultAudioCallAttachment> _onDisposed;
    private readonly object _sync = new();

    private bool _started;
    private bool _connected;
    private bool _disposed;

    internal DefaultAudioCallAttachment(
        ICall call,
        MediaManager mediaManager,
        IAudioDevice audioDevice,
        ILoggerFactory loggerFactory,
        Action<CallId, DefaultAudioCallAttachment> onDisposed)
    {
        _call = call ?? throw new ArgumentNullException(nameof(call));
        _audioDevice = audioDevice ?? throw new ArgumentNullException(nameof(audioDevice));
        ArgumentNullException.ThrowIfNull(mediaManager);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _onDisposed = onDisposed ?? throw new ArgumentNullException(nameof(onDisposed));

        _receiver = mediaManager.CreateReceiver();
        _sender = mediaManager.CreateSender();
        _logger = loggerFactory.CreateLogger<DefaultAudioCallAttachment>();
    }

    internal void EnsureStarted()
    {
        lock (_sync)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DefaultAudioCallAttachment));

            if (!_started)
            {
                _call.StateChanged += OnCallStateChanged;
                _started = true;
            }
        }

        ApplyState(_call.State, throwOnConnectFailure: true);
    }

    private void OnCallStateChanged(object? sender, CallStateChangedEventArgs args)
    {
        try
        {
            ApplyState(args.NewState, throwOnConnectFailure: false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Default audio transition failed for call {CallId} on state {CallState}.",
                _call.CallId,
                args.NewState);
        }
    }

    private void ApplyState(CallState state, bool throwOnConnectFailure)
    {
        if (state == CallState.Terminated)
        {
            Dispose();
            return;
        }

        if (state is not (CallState.Connected or CallState.OnHold))
            return;

        ConnectIfNeeded(throwOnConnectFailure);
    }

    private void ConnectIfNeeded(bool throwOnConnectFailure)
    {
        AudioConnectionParameters parameters;
        lock (_sync)
        {
            if (_disposed || _connected)
                return;

            parameters = _call.MediaParameters is { } mediaParameters
                ? AudioConnectionParameters.From(mediaParameters)
                : AudioConnectionParameters.Default;
        }

        try
        {
            _receiver.AttachToCall(_call);
            _sender.AttachToCall(_call);
            _audioDevice.Connect(_receiver, _sender, parameters);

            lock (_sync)
            {
                if (!_disposed)
                    _connected = true;
            }

            _logger.LogDebug(
                "Default audio connected for call {CallId} with PT={PayloadType} SR={SampleRate}.",
                _call.CallId,
                parameters.PayloadType,
                parameters.SampleRate);
        }
        catch (Exception ex)
        {
            lock (_sync)
            {
                if (!_disposed)
                    _connected = false;
            }

            TryDetachMediaLegs();
            _logger.LogWarning(ex, "Default audio connect failed for call {CallId}.", _call.CallId);
            if (throwOnConnectFailure)
                throw;
        }
    }

    public void Dispose()
    {
        bool wasStarted;
        bool wasConnected;
        lock (_sync)
        {
            if (_disposed)
                return;

            _disposed = true;
            wasStarted = _started;
            wasConnected = _connected;
            _started = false;
            _connected = false;
        }

        if (wasStarted)
            _call.StateChanged -= OnCallStateChanged;

        if (wasConnected)
        {
            try
            {
                _audioDevice.Disconnect();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Default audio disconnect failed for call {CallId}.", _call.CallId);
            }
        }

        TryDetachMediaLegs();

        try
        {
            _receiver.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Disposing media receiver failed for call {CallId}.", _call.CallId);
        }

        try
        {
            _sender.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Disposing media sender failed for call {CallId}.", _call.CallId);
        }

        _onDisposed(_call.CallId, this);
    }

    private void TryDetachMediaLegs()
    {
        try
        {
            _receiver.Detach();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Detaching media receiver failed for call {CallId}.", _call.CallId);
        }

        try
        {
            _sender.Detach();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Detaching media sender failed for call {CallId}.", _call.CallId);
        }
    }
}
