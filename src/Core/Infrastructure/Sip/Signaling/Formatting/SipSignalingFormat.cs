using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Common.Network;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Formatting utilities for SIP signaling headers.
/// </summary>
internal static class SipSignalingFormat
{
    /// <summary>
    /// Builds a Via header value for local transport endpoint and branch token.
    /// </summary>
    public static string BuildVia(
        IPEndPoint localEndPoint,
        string branch,
        SipTransportProtocol transport)
    {
        var host = LocalEndPointHostResolver.ResolveHost(localEndPoint);
        var transportToken = transport switch
        {
            SipTransportProtocol.Tcp => "TCP",
            SipTransportProtocol.Tls => "TLS",
            SipTransportProtocol.Ws => "WS",
            SipTransportProtocol.Wss => "WSS",
            _ => "UDP"
        };

        return $"SIP/2.0/{transportToken} {host}:{localEndPoint.Port};branch={branch};rport";
    }

    /// <summary>
    /// Builds a Contact URI for the local endpoint and user.
    /// </summary>
    public static string BuildContactUri(
        string username,
        IPEndPoint localEndPoint,
        SipTransportProtocol transport,
        bool forceSecureScheme = false)
    {
        var host = LocalEndPointHostResolver.ResolveHost(localEndPoint);
        var scheme = forceSecureScheme || transport is SipTransportProtocol.Tls or SipTransportProtocol.Wss
            ? "sips"
            : "sip";

        var transportParam = transport switch
        {
            SipTransportProtocol.Tcp => ";transport=tcp",
            SipTransportProtocol.Tls => ";transport=tls",
            SipTransportProtocol.Ws => ";transport=ws",
            SipTransportProtocol.Wss => ";transport=wss",
            _ => ";transport=udp"
        };

        return $"{scheme}:{username}@{host}:{localEndPoint.Port}{transportParam}";
    }
}
