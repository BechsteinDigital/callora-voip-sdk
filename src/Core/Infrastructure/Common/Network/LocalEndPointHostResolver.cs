using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Common.Network;

/// <summary>
/// Resolves a stable host string from local bind endpoints.
/// Converts wildcard addresses to loopback for deterministic local signaling payloads.
/// </summary>
internal static class LocalEndPointHostResolver
{
    /// <summary>
    /// Resolves a host value that can be embedded in protocol headers/bodies.
    /// </summary>
    public static string ResolveHost(IPEndPoint localEndPoint)
    {
        ArgumentNullException.ThrowIfNull(localEndPoint);

        if (IPAddress.Any.Equals(localEndPoint.Address)
            || IPAddress.IPv6Any.Equals(localEndPoint.Address))
        {
            return IPAddress.Loopback.ToString();
        }

        return localEndPoint.Address.ToString();
    }
}
