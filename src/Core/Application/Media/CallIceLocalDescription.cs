using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Local ICE credentials and candidates generated for one call leg.
/// </summary>
internal sealed class CallIceLocalDescription
{
    /// <summary>
    /// Generated local ICE username fragment.
    /// </summary>
    public required string Ufrag { get; init; }

    /// <summary>
    /// Generated local ICE password.
    /// </summary>
    public required string Pwd { get; init; }

    /// <summary>
    /// Optional ICE options value for local SDP.
    /// </summary>
    public string? Options { get; init; }

    /// <summary>
    /// Gathered local ICE candidates.
    /// </summary>
    public IReadOnlyList<CallIceCandidate> Candidates { get; init; } = [];
}
