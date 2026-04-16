using System.Net;
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Ports.Connectivity;
using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;
using CalloraVoipSdk;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Infrastructure adapter exposing TURN relay allocation to the application ICE agent.
/// </summary>
internal sealed class TurnIceRelayAllocator : IIceTurnRelayAllocator
{
    private readonly ITurnClient _turnClient;
    private readonly ILogger<TurnIceRelayAllocator> _logger;

    /// <summary>
    /// Creates a TURN relay allocator adapter.
    /// </summary>
    internal TurnIceRelayAllocator(
        ITurnClient turnClient,
        ILoggerFactory loggerFactory)
    {
        _turnClient = turnClient ?? throw new ArgumentNullException(nameof(turnClient));
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _logger = loggerFactory.CreateLogger<TurnIceRelayAllocator>();
    }

    /// <inheritdoc />
    public async Task<IceRelayAllocation?> TryAllocateRelayAsync(
        IPEndPoint localEndPoint,
        IceServerConfiguration server,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(localEndPoint);
        ArgumentNullException.ThrowIfNull(server);

        if (server.Type != IceServerType.Turn)
            return null;

        try
        {
            var transport = MapTransport(server.Transport);
            var serverEndPoint = await ResolveServerEndPointAsync(server, transport, ct).ConfigureAwait(false);
            var result = await _turnClient
                .AllocateAsync(
                    serverEndPoint,
                    BuildCredentials(server),
                    BuildAllocateOptions(localEndPoint),
                    transport,
                    tlsTargetHost: server.Host,
                    ct: ct)
                .ConfigureAwait(false);

            return new IceRelayAllocation
            {
                RelayedEndPoint = result.RelayedEndPoint,
                MappedEndPoint = result.MappedEndPoint,
                Lifetime = TimeSpan.FromSeconds(Math.Max(0, result.LifetimeSeconds))
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogDebug(
                ex,
                "ICE TURN relay allocation failed for {Host}:{Port}.",
                server.Host,
                server.Port ?? 0);
            return null;
        }
    }

    private static TurnAllocateOptions BuildAllocateOptions(IPEndPoint localEndPoint)
        => new()
        {
            RequestedTransport = TurnRequestedTransportProtocol.Udp,
            RequestedAddressFamily = localEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                ? TurnAddressFamily.IPv6
                : TurnAddressFamily.IPv4
        };

    private static StunCredentials? BuildCredentials(IceServerConfiguration server)
    {
        if (string.IsNullOrWhiteSpace(server.Username) || string.IsNullOrWhiteSpace(server.Password))
            return null;

        return new StunCredentials
        {
            Username = server.Username,
            Password = server.Password
        };
    }

    private static async Task<IPEndPoint> ResolveServerEndPointAsync(
        IceServerConfiguration server,
        TurnTransport transport,
        CancellationToken ct)
    {
        var port = server.Port ?? DefaultPortFor(transport);
        if (IPAddress.TryParse(server.Host, out var ipAddress))
            return new IPEndPoint(ipAddress, port);

        var addresses = await Dns.GetHostAddressesAsync(server.Host, ct).ConfigureAwait(false);
        var firstAddress = addresses.FirstOrDefault()
            ?? throw new InvalidOperationException($"Unable to resolve TURN host '{server.Host}'.");
        return new IPEndPoint(firstAddress, port);
    }

    private static int DefaultPortFor(TurnTransport transport)
        => transport == TurnTransport.Tls ? 5349 : 3478;

    private static TurnTransport MapTransport(IceTransport transport)
        => transport switch
        {
            IceTransport.Udp => TurnTransport.Udp,
            IceTransport.Tcp => TurnTransport.Tcp,
            IceTransport.Tls => TurnTransport.Tls,
            _ => throw new ArgumentOutOfRangeException(nameof(transport), transport, "Unsupported ICE transport.")
        };
}
