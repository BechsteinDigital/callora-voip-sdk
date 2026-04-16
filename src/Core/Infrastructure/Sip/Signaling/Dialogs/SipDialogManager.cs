using CalloraVoipSdk.Core.Infrastructure.Common.Protocols;
using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Tracks early and confirmed dialog routing state including route-set and remote target refresh.
/// </summary>
internal sealed class SipDialogManager
{
    private readonly object _sync = new();
    private readonly Dictionary<string, SipDialogPath> _earlyDialogs = new(StringComparer.Ordinal);
    private readonly Dictionary<string, SipDialogPath> _confirmedDialogs = new(StringComparer.Ordinal);
    private string? _latestEarlyDialogTag;
    private string? _activeConfirmedDialogTag;
    private string? _latestConfirmedDialogTag;

    /// <summary>
    /// Current active confirmed remote tag when available.
    /// </summary>
    public string? ConfirmedRemoteTag
    {
        get
        {
            lock (_sync)
                return TryGetActiveConfirmedDialogLocked()?.RemoteTag
                    ?? TryGetLatestConfirmedDialogLocked()?.RemoteTag;
        }
    }

    /// <summary>
    /// Current in-dialog request target URI.
    /// </summary>
    public string? RemoteTargetUri
    {
        get
        {
            lock (_sync)
                return TryGetActiveConfirmedDialogLocked()?.RemoteTargetUri
                    ?? TryGetLatestConfirmedDialogLocked()?.RemoteTargetUri
                    ?? TryGetLatestEarlyDialogLocked()?.RemoteTargetUri;
        }
    }

    /// <summary>
    /// Current route set for in-dialog requests.
    /// </summary>
    public IReadOnlyList<string> RouteSet
    {
        get
        {
            lock (_sync)
                return TryGetActiveConfirmedDialogLocked()?.RouteSet
                    ?? TryGetLatestConfirmedDialogLocked()?.RouteSet
                    ?? TryGetLatestEarlyDialogLocked()?.RouteSet
                    ?? [];
        }
    }

    /// <summary>
    /// Number of currently tracked confirmed dialogs.
    /// Used by compliance tests and diagnostics.
    /// </summary>
    internal int ConfirmedDialogCount
    {
        get
        {
            lock (_sync)
                return _confirmedDialogs.Count;
        }
    }

    /// <summary>
    /// Updates early/confirmed dialog state from an INVITE transaction response.
    /// </summary>
    public void ApplyInviteResponse(
        SipResponse response,
        string fallbackRemoteUri)
    {
        var toTag = SipProtocol.ExtractTag(response.Header("To"));
        if (string.IsNullOrWhiteSpace(toTag))
            return;

        var routeSet = ParseRouteSetFromRecordRoute(response.Header("Record-Route"));
        var remoteTarget = ExtractRemoteTarget(response, fallbackRemoteUri);
        var path = new SipDialogPath
        {
            RemoteTag = toTag,
            RemoteTargetUri = remoteTarget,
            RouteSet = routeSet
        };

        lock (_sync)
        {
            if (SipProtocol.IsProvisional(response.StatusCode))
            {
                _earlyDialogs[toTag] = path;
                _latestEarlyDialogTag = toTag;
                return;
            }

            if (SipProtocol.IsSuccess(response.StatusCode))
            {
                var confirmedPath = _earlyDialogs.TryGetValue(toTag, out var early)
                    ? new SipDialogPath
                    {
                        RemoteTag = early.RemoteTag,
                        RemoteTargetUri = remoteTarget,
                        RouteSet = early.RouteSet
                    }
                    : path;

                _confirmedDialogs[toTag] = confirmedPath;
                _latestConfirmedDialogTag = toTag;
                _activeConfirmedDialogTag ??= toTag;
                _earlyDialogs.Remove(toTag);
                if (string.Equals(_latestEarlyDialogTag, toTag, StringComparison.Ordinal))
                    _latestEarlyDialogTag = _earlyDialogs.Count > 0 ? _earlyDialogs.Keys.Last() : null;
                return;
            }

            _earlyDialogs.Remove(toTag);
            if (string.Equals(_latestEarlyDialogTag, toTag, StringComparison.Ordinal))
                _latestEarlyDialogTag = _earlyDialogs.Count > 0 ? _earlyDialogs.Keys.Last() : null;
        }
    }

    /// <summary>
    /// Updates dialog state from inbound request and performs target refresh when Contact is present.
    /// </summary>
    public void ApplyInboundRequest(
        SipRequest request,
        string fallbackRemoteUri)
    {
        var remoteTag = SipProtocol.ExtractTag(request.Header("From"));
        if (string.IsNullOrWhiteSpace(remoteTag))
            return;

        var remoteTarget = ExtractRemoteTarget(request, fallbackRemoteUri);
        var routeSet = ParseRouteSetFromRecordRoute(request.Header("Record-Route"));
        lock (_sync)
        {
            if (!_confirmedDialogs.TryGetValue(remoteTag, out var confirmedPath))
            {
                confirmedPath = new SipDialogPath
                {
                    RemoteTag = remoteTag,
                    RemoteTargetUri = remoteTarget,
                    RouteSet = routeSet
                };
                _confirmedDialogs[remoteTag] = confirmedPath;
            }

            _latestConfirmedDialogTag = remoteTag;
            _activeConfirmedDialogTag = remoteTag;

            if (IsTargetRefreshMethod(request.Method))
            {
                confirmedPath.RemoteTargetUri = remoteTarget;
            }
        }
    }

    /// <summary>
    /// Updates remote target from successful target-refresh responses.
    /// </summary>
    public void ApplyTargetRefreshResponse(
        SipResponse response,
        string method,
        string fallbackRemoteUri)
    {
        if (!SipProtocol.IsSuccess(response.StatusCode))
            return;
        if (!IsTargetRefreshMethod(method))
            return;

        var remoteTarget = ExtractRemoteTarget(response, fallbackRemoteUri);
        var remoteTag = SipProtocol.ExtractTag(response.Header("To"));
        lock (_sync)
        {
            if (!string.IsNullOrWhiteSpace(remoteTag)
                && _confirmedDialogs.TryGetValue(remoteTag, out var taggedPath))
            {
                taggedPath.RemoteTargetUri = remoteTarget;
                return;
            }

            var active = TryGetActiveConfirmedDialogLocked();
            if (active is not null)
                active.RemoteTargetUri = remoteTarget;
        }
    }

    /// <summary>
    /// Extracts remote target URI from Contact header with fallback.
    /// </summary>
    private static string ExtractRemoteTarget(SipRequest request, string fallbackRemoteUri)
    {
        var contact = request.Header("Contact");
        var uri = SipProtocol.ExtractUriFromNameAddr(contact);
        return string.IsNullOrWhiteSpace(uri) ? fallbackRemoteUri : uri;
    }

    /// <summary>
    /// Extracts remote target URI from response Contact header with fallback.
    /// </summary>
    private static string ExtractRemoteTarget(SipResponse response, string fallbackRemoteUri)
    {
        var contact = response.Header("Contact");
        var uri = SipProtocol.ExtractUriFromNameAddr(contact);
        return string.IsNullOrWhiteSpace(uri) ? fallbackRemoteUri : uri;
    }

    /// <summary>
    /// Parses route-set from Record-Route header list.
    /// UAC side stores reverse order according RFC3261.
    /// </summary>
    private static IReadOnlyList<string> ParseRouteSetFromRecordRoute(string? recordRoute)
    {
        if (string.IsNullOrWhiteSpace(recordRoute))
            return [];

        var routes = ProtocolCommonUtilities
            .SplitCommaSeparatedRespectingQuotes(recordRoute)
            .Select(SipProtocol.ExtractUriFromNameAddr)
            .Where(uri => !string.IsNullOrWhiteSpace(uri))
            .ToList();
        routes.Reverse();
        return routes;
    }

    /// <summary>
    /// Returns true for target-refresh methods.
    /// </summary>
    private static bool IsTargetRefreshMethod(string method)
    {
        var normalized = method.Trim().ToUpperInvariant();
        return normalized is "INVITE" or "UPDATE";
    }

    /// <summary>
    /// Returns latest early dialog path when available.
    /// </summary>
    private SipDialogPath? TryGetLatestEarlyDialogLocked()
    {
        if (string.IsNullOrWhiteSpace(_latestEarlyDialogTag))
            return null;
        return _earlyDialogs.TryGetValue(_latestEarlyDialogTag, out var path) ? path : null;
    }

    /// <summary>
    /// Returns active confirmed dialog path when available.
    /// </summary>
    private SipDialogPath? TryGetActiveConfirmedDialogLocked()
    {
        if (string.IsNullOrWhiteSpace(_activeConfirmedDialogTag))
            return null;
        return _confirmedDialogs.TryGetValue(_activeConfirmedDialogTag, out var path) ? path : null;
    }

    /// <summary>
    /// Returns latest confirmed dialog path when available.
    /// </summary>
    private SipDialogPath? TryGetLatestConfirmedDialogLocked()
    {
        if (string.IsNullOrWhiteSpace(_latestConfirmedDialogTag))
            return null;
        return _confirmedDialogs.TryGetValue(_latestConfirmedDialogTag, out var path) ? path : null;
    }
}
