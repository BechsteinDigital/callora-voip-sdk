using CalloraVoipSdk.Core.Application.Calls;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;
using CalloraVoipSdk.Modules;

namespace CalloraVoipSdk;

/// <summary>
/// Read-only runtime view over active calls and media sessions.
/// </summary>
public sealed class SessionManager
{
    private readonly CallManager _calls;
    private readonly IConferencingModule _conferencing;
    private readonly IPlaybackModule _playback;
    private readonly IRecordingModule _recording;

    internal SessionManager(
        CallManager calls,
        IConferencingModule conferencing,
        IPlaybackModule playback,
        IRecordingModule recording)
    {
        _calls = calls;
        _conferencing = conferencing;
        _playback = playback;
        _recording = recording;
    }

    /// <summary>
    /// Active non-terminated calls.
    /// </summary>
    public IReadOnlyCollection<ICall> ActiveCalls => _calls.Active;

    /// <summary>
    /// Active conferences (empty when module unavailable).
    /// </summary>
    public IReadOnlyCollection<IConferenceSession> ActiveConferences => _conferencing.Active;

    /// <summary>
    /// Active playback sessions (empty when module unavailable).
    /// </summary>
    public IReadOnlyCollection<IPlaybackSession> ActivePlaybacks => _playback.Active;

    /// <summary>
    /// Active recording sessions (empty when module unavailable).
    /// </summary>
    public IReadOnlyCollection<IRecordingSession> ActiveRecordings => _recording.Active;
}
