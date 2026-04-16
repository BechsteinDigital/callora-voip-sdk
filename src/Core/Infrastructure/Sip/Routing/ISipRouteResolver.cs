namespace CalloraVoipSdk.Core.Infrastructure.Sip.Routing;

/// <summary>
/// Resolves SIP destinations into concrete network route candidates.
/// </summary>
internal interface ISipRouteResolver
{
    /// <summary>
    /// Resolves one SIP destination using RFC3263-inspired DNS fallback.
    /// </summary>
    Task<SipRouteResolutionResult> ResolveAsync(
        SipRouteResolutionRequest request,
        CancellationToken ct = default);
}

