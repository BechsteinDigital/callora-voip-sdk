using CalloraVoipSdk.Core.Infrastructure.Common.Protocols;
using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

internal static class SipCallSessionTransactionUtilities
{
    public static IReadOnlyList<string> ParseRouteSetFromRecordRoute(string? recordRoute)
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

    public static string AppendSupportedToken(string? supportedHeader, string token)
    {
        if (string.IsNullOrWhiteSpace(supportedHeader))
            return token;

        return ProtocolCommonUtilities.ContainsToken(supportedHeader, token)
            ? supportedHeader
            : $"{supportedHeader}, {token}";
    }

    public static IReadOnlyDictionary<string, string>? CreateReasonHeaders(SipDialogTerminationReason? reason)
    {
        if (reason is null)
            return null;

        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Reason"] = SipReasonHeader.Format(reason)
        };
    }

    public static void AppendReasonHeader(IDictionary<string, string> headers, SipDialogTerminationReason? reason)
    {
        if (reason is null)
            return;

        headers["Reason"] = SipReasonHeader.Format(reason);
    }

    public static SipDialogTerminationReason ResolveTerminationReason(
        string? reasonHeader,
        int statusCode,
        string reasonPhrase,
        int? retryAfterSeconds = null)
    {
        var reason = SipReasonHeader.TryParseFirst(reasonHeader, out var parsedReason) && parsedReason is not null
            ? parsedReason
            : SipReasonHeader.CreateSipStatusReason(statusCode, reasonPhrase);

        if (retryAfterSeconds.HasValue)
            return new SipDialogTerminationReason(reason.Protocol, reason.Cause, reason.Text, retryAfterSeconds);

        return reason;
    }
}
