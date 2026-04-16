using System.Net;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Application-level ICE orchestration port.
/// Responsible for local candidate gathering and candidate-pair selection.
/// </summary>
internal interface ICallIceAgent
{
    /// <summary>
    /// Builds local ICE credentials and candidates for one SDP offer/answer exchange.
    /// </summary>
    Task<CallIceLocalDescription?> BuildLocalDescriptionAsync(
        IPEndPoint localEndPoint,
        CancellationToken ct = default);

    /// <summary>
    /// Runs ICE connectivity checks and returns the selected candidate pair.
    /// </summary>
    Task<CallIceSelectionResult> SelectCandidatePairAsync(
        CallId callId,
        CallMediaParameters parameters,
        CancellationToken ct = default);
}
