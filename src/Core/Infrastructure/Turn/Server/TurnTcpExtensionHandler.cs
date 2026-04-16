using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// RFC 6062 request handling for CONNECT and CONNECTION-BIND.
/// </summary>
internal sealed class TurnTcpExtensionHandler
{
    private readonly ConcurrentDictionary<string, TurnServerAllocation> _allocationsByClient;
    private readonly TurnServerResponseFactory _responseFactory;
    private readonly TurnTcpConnectionBroker _connectionBroker;
    private readonly Func<string, TurnServerAllocation?> _liveAllocationLookup;
    private readonly ILogger<TurnServer> _logger;

    /// <summary>
    /// Creates a handler for TCP-oriented TURN extensions.
    /// </summary>
    public TurnTcpExtensionHandler(
        ConcurrentDictionary<string, TurnServerAllocation> allocationsByClient,
        TurnServerResponseFactory responseFactory,
        TurnTcpConnectionBroker connectionBroker,
        Func<string, TurnServerAllocation?> liveAllocationLookup,
        ILogger<TurnServer> logger)
    {
        ArgumentNullException.ThrowIfNull(allocationsByClient);
        ArgumentNullException.ThrowIfNull(responseFactory);
        ArgumentNullException.ThrowIfNull(connectionBroker);
        ArgumentNullException.ThrowIfNull(liveAllocationLookup);
        ArgumentNullException.ThrowIfNull(logger);

        _allocationsByClient = allocationsByClient;
        _responseFactory = responseFactory;
        _connectionBroker = connectionBroker;
        _liveAllocationLookup = liveAllocationLookup;
        _logger = logger;
    }

    /// <summary>
    /// Handles RFC 6062 CONNECT by creating an outgoing TCP connection to the peer.
    /// </summary>
    public async Task<StunMessage> HandleConnectRequestAsync(
        StunMessage request,
        TurnClientContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var allocation = _liveAllocationLookup(context.ClientKey);
        if (allocation is null)
            return _responseFactory.BuildErrorResponse(request, 437, "Allocation Mismatch", includeAuthAttributes: false);

        if (allocation.RelayedTransport != TurnRequestedTransportProtocol.Tcp)
            return _responseFactory.BuildErrorResponse(request, 442, "Unsupported Transport Protocol", includeAuthAttributes: false);

        var peer = TurnAttributeMapper.DecodeXorPeerAddress(request)?.EndPoint;
        if (peer is null)
            return _responseFactory.BuildErrorResponse(request, 400, "Bad Request", includeAuthAttributes: false);

        if (!TurnMobilityService.IsPeerFamilyMatchingAllocation(allocation, peer))
            return _responseFactory.BuildErrorResponse(request, 443, "Peer Address Family Mismatch", includeAuthAttributes: false);

        if (!allocation.HasValidPermission(peer, DateTimeOffset.UtcNow))
            return _responseFactory.BuildErrorResponse(request, 403, "Forbidden", includeAuthAttributes: false);

        uint connectionId;
        try
        {
            connectionId = await _connectionBroker.ConnectPeerAsync(allocation.ClientKey, peer, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is SocketException or IOException)
        {
            _logger.LogWarning(ex, "TURN CONNECT to peer {Peer} failed", peer);
            return _responseFactory.BuildErrorResponse(request, 447, "Connection Timeout or Failure", includeAuthAttributes: false);
        }

        return _responseFactory.BuildSuccessResponse(
            request,
            [
                TurnAttributeMapper.Encode(new TurnConnectionIdAttribute
                {
                    ConnectionId = connectionId
                })
            ]);
    }

    /// <summary>
    /// Handles RFC 6062 CONNECTION-BIND by attaching a pending connection to one client stream.
    /// </summary>
    public StunMessage HandleConnectionBindRequest(StunMessage request, TurnClientContext context)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (context.StreamConnection is null)
            return _responseFactory.BuildErrorResponse(request, 400, "Bad Request", includeAuthAttributes: false);

        var connectionId = TurnAttributeMapper.DecodeConnectionId(request)?.ConnectionId;
        if (!connectionId.HasValue)
            return _responseFactory.BuildErrorResponse(request, 400, "Bad Request", includeAuthAttributes: false);

        if (!_connectionBroker.TryBind(connectionId.Value, context.StreamConnection.Id, out var allocationKey))
            return _responseFactory.BuildErrorResponse(request, 400, "Bad Request", includeAuthAttributes: false);

        if (!_allocationsByClient.ContainsKey(allocationKey) || _liveAllocationLookup(allocationKey) is null)
        {
            _connectionBroker.RemoveByClientStream(context.StreamConnection.Id);
            return _responseFactory.BuildErrorResponse(request, 437, "Allocation Mismatch", includeAuthAttributes: false);
        }

        return _responseFactory.BuildSuccessResponse(request, []);
    }
}
