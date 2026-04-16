namespace CalloraVoipSdk.Core.Infrastructure.Sip.Routing;

/// <summary>
/// Result container for one SIP route resolution operation.
/// Candidates are ordered by preference from best to fallback.
/// </summary>
internal sealed class SipRouteResolutionResult
{
    /// <summary>
    /// Ordered candidate list returned by the resolver.
    /// </summary>
    public required IReadOnlyList<SipRouteCandidate> Candidates { get; init; }

    /// <summary>
    /// Returns the highest-priority route candidate.
    /// </summary>
    public SipRouteCandidate Primary =>
        Candidates.Count > 0
            ? Candidates[0]
            : throw new InvalidOperationException("Route resolution returned no candidates.");
}

