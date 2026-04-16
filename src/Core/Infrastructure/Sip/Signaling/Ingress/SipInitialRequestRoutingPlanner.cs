using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Builds initial outbound INVITE request routing according to preloaded route-set semantics.
/// </summary>
internal static class SipInitialRequestRoutingPlanner
{
    /// <summary>
    /// Creates the initial outbound target from remote URI and optional preloaded route-set.
    /// </summary>
    public static SipOutboundInviteTarget CreateInitialTarget(
        string remoteUri,
        IReadOnlyList<string>? preloadedRouteSet)
    {
        if (string.IsNullOrWhiteSpace(remoteUri))
            throw new ArgumentException("Remote URI is required.", nameof(remoteUri));
        if (!SipProtocol.TryParseSipUri(remoteUri, out _, out _, out _))
            throw new ArgumentException($"Remote URI must be a valid SIP URI, got '{remoteUri}'.", nameof(remoteUri));

        var normalizedRouteSet = NormalizeRouteSet(preloadedRouteSet);
        if (normalizedRouteSet.Count == 0)
        {
            return new SipOutboundInviteTarget(
                RequestUri: remoteUri,
                LogicalRemoteUri: remoteUri,
                RouteSet: [],
                NextHopUri: remoteUri);
        }

        var firstRoute = normalizedRouteSet[0];
        if (HasLooseRoutingParameter(firstRoute))
        {
            return new SipOutboundInviteTarget(
                RequestUri: remoteUri,
                LogicalRemoteUri: remoteUri,
                RouteSet: normalizedRouteSet,
                NextHopUri: firstRoute);
        }

        var strictRouteSet = normalizedRouteSet.Skip(1).Concat([remoteUri]).ToArray();
        return new SipOutboundInviteTarget(
            RequestUri: firstRoute,
            LogicalRemoteUri: remoteUri,
            RouteSet: strictRouteSet,
            NextHopUri: firstRoute);
    }

    /// <summary>
    /// Normalizes one configured route-set into plain SIP URIs and validates each entry.
    /// </summary>
    private static IReadOnlyList<string> NormalizeRouteSet(IReadOnlyList<string>? preloadedRouteSet)
    {
        if (preloadedRouteSet is null || preloadedRouteSet.Count == 0)
            return [];

        var normalized = new List<string>(preloadedRouteSet.Count);
        foreach (var route in preloadedRouteSet)
        {
            if (string.IsNullOrWhiteSpace(route))
                continue;

            var uri = ExtractRouteUri(route);
            if (!SipProtocol.TryParseSipUri(uri, out _, out _, out _))
                throw new ArgumentException($"Route-set entry must be a valid SIP URI, got '{route}'.", nameof(preloadedRouteSet));

            normalized.Add(uri);
        }

        return normalized;
    }

    /// <summary>
    /// Extracts URI from route value while preserving URI parameters on plain URI values.
    /// </summary>
    private static string ExtractRouteUri(string routeValue)
    {
        var trimmed = routeValue.Trim();
        if (trimmed.IndexOf('<') >= 0 && trimmed.IndexOf('>') > trimmed.IndexOf('<'))
            return SipProtocol.ExtractUriFromNameAddr(trimmed);
        return trimmed;
    }

    /// <summary>
    /// Returns true when route URI includes the loose-routing <c>lr</c> parameter.
    /// </summary>
    private static bool HasLooseRoutingParameter(string routeUri)
    {
        if (string.IsNullOrWhiteSpace(routeUri))
            return false;

        var value = routeUri;
        var headerIndex = value.IndexOf('?');
        if (headerIndex >= 0)
            value = value[..headerIndex];

        var parameterIndex = value.IndexOf(';');
        if (parameterIndex < 0)
            return false;

        var parameters = value[(parameterIndex + 1)..]
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var parameter in parameters)
        {
            if (parameter.Equals("lr", StringComparison.OrdinalIgnoreCase))
                return true;

            if (parameter.StartsWith("lr=", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
