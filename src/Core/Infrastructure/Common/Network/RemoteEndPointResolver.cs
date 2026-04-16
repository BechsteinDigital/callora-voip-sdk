using System.Net;
using System.Net.Sockets;

namespace CalloraVoipSdk.Core.Infrastructure.Common.Network;

/// <summary>
/// Resolves host and port pairs into concrete remote IP endpoints.
/// Prefers IPv4 to align with common VoIP transport expectations.
/// </summary>
internal static class RemoteEndPointResolver
{
    /// <summary>
    /// Resolves one host/port target into an endpoint.
    /// </summary>
    public static async Task<IPEndPoint> ResolveAsync(
        string host,
        int port,
        CancellationToken ct = default)
    {
        if (IPAddress.TryParse(host, out var ipAddress))
            return new IPEndPoint(ipAddress, port);

        var addresses = await Dns.GetHostAddressesAsync(host, ct).ConfigureAwait(false);
        var selected = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                       ?? addresses.First();
        return new IPEndPoint(selected, port);
    }
}
