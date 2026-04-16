using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Application port: creates RTP media sessions from negotiated SDP parameters.
/// Implemented by infrastructure; injected into <see cref="CallMediaOrchestrator"/>.
/// </summary>
internal interface ICallMediaSessionFactory
{
    /// <summary>
    /// Creates a media session for the given negotiated parameters.
    /// The session is not started; call <see cref="ICallMediaSession.StartAsync"/> explicitly.
    /// </summary>
    ICallMediaSession Create(CallMediaParameters parameters);
}
