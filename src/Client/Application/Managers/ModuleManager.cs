using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Modules;

namespace CalloraVoipSdk;

/// <summary>
/// Runtime availability facade for optional SDK modules.
/// </summary>
public sealed class ModuleManager
{
    internal ModuleManager(
        MediaManager media,
        ILoggerFactory loggerFactory)
    {
        Conferencing = ModuleAdapters.CreateConferencing(media, loggerFactory);
        Playback = ModuleAdapters.CreatePlayback(media);
        Recording = ModuleAdapters.CreateRecording(media);
        Realtime = ModuleAdapters.CreateRealtime(media, loggerFactory);
        WebSocket = ModuleAdapters.CreateWebSocketAudioTransport(loggerFactory);
    }

    /// <summary>
    /// Current conferencing module facade.
    /// </summary>
    public IConferencingModule Conferencing { get; }

    /// <summary>
    /// Current playback module facade.
    /// </summary>
    public IPlaybackModule Playback { get; }

    /// <summary>
    /// Current recording module facade.
    /// </summary>
    public IRecordingModule Recording { get; }

    /// <summary>
    /// Current realtime bridge module facade.
    /// </summary>
    public IRealtimeModule Realtime { get; }

    /// <summary>
    /// Current websocket module facade.
    /// </summary>
    public IWebSocketModule WebSocket { get; }

    /// <summary>
    /// True when conferencing is available.
    /// </summary>
    public bool ConferencingAvailable => Conferencing.IsAvailable;

    /// <summary>
    /// True when playback is available.
    /// </summary>
    public bool PlaybackAvailable => Playback.IsAvailable;

    /// <summary>
    /// True when recording is available.
    /// </summary>
    public bool RecordingAvailable => Recording.IsAvailable;

    /// <summary>
    /// True when realtime bridge is available.
    /// </summary>
    public bool RealtimeAvailable => Realtime.IsAvailable;

    /// <summary>
    /// True when websocket transport module is available.
    /// </summary>
    public bool WebSocketAvailable => WebSocket.IsAvailable;
}
