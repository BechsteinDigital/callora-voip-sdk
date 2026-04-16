using System.Net;
using System.Security.Cryptography;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;
using CalloraVoipSdk.Core.Infrastructure.Stun.Wire;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Handles RFC 6062 passive peer TCP connections and emits CONNECTION-ATTEMPT indications.
/// </summary>
internal sealed class TurnTcpPassiveConnectionService
{
    private readonly IStunMessageCodec _codec;
    private readonly TurnTcpConnectionBroker _connectionBroker;
    private readonly ILogger<TurnServer> _logger;

    /// <summary>
    /// Creates passive TCP connection service.
    /// </summary>
    public TurnTcpPassiveConnectionService(
        IStunMessageCodec codec,
        TurnTcpConnectionBroker connectionBroker,
        ILogger<TurnServer> logger)
    {
        ArgumentNullException.ThrowIfNull(codec);
        ArgumentNullException.ThrowIfNull(connectionBroker);
        ArgumentNullException.ThrowIfNull(logger);

        _codec = codec;
        _connectionBroker = connectionBroker;
        _logger = logger;
    }

    /// <summary>
    /// Runs accept loop for one TCP allocation and forwards CONNECTION-ATTEMPT indications.
    /// </summary>
    public async Task RunAsync(
        TurnServerAllocation allocation,
        Func<TurnServerAllocation, byte[], CancellationToken, Task> sendToClientAsync,
        CancellationToken serverStop)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        ArgumentNullException.ThrowIfNull(sendToClientAsync);

        var listener = allocation.RelayTcpListener;
        var relayStop = allocation.RelayTcpStop;
        if (listener is null || relayStop is null)
            return;

        using var linkedStop = CancellationTokenSource.CreateLinkedTokenSource(serverStop, relayStop.Token);
        var ct = linkedStop.Token;

        while (!ct.IsCancellationRequested)
        {
            TcpClient peerClient;
            try
            {
                peerClient = await listener.AcceptTcpClientAsync(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TURN TCP relay accept failed for client {Client}", allocation.ClientKey);
                continue;
            }

            var peerEndPoint = peerClient.Client.RemoteEndPoint as IPEndPoint;
            if (peerEndPoint is null)
            {
                peerClient.Dispose();
                continue;
            }

            var now = DateTimeOffset.UtcNow;
            if (now > allocation.ExpiresAtUtc || !allocation.HasValidPermission(peerEndPoint, now))
            {
                peerClient.Dispose();
                continue;
            }

            var connectionId = _connectionBroker.RegisterIncomingPeer(allocation.ClientKey, peerEndPoint, peerClient);
            var indicationBytes = BuildConnectionAttemptIndication(connectionId, peerEndPoint);

            try
            {
                await sendToClientAsync(allocation, indicationBytes, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "TURN failed to send CONNECTION-ATTEMPT to client {Client}", allocation.ClientKey);
                _connectionBroker.RemovePending(connectionId);
            }
        }
    }

    private byte[] BuildConnectionAttemptIndication(uint connectionId, IPEndPoint peerEndPoint)
    {
        var txId = new byte[StunWireConstants.TransactionIdLength];
        RandomNumberGenerator.Fill(txId);

        var indication = new StunMessage
        {
            MessageClass = StunMessageClass.Indication,
            MessageMethod = (StunMessageMethod)(ushort)TurnMessageMethod.ConnectionAttempt,
            TransactionId = txId,
            Attributes =
            [
                TurnAttributeMapper.Encode(new TurnConnectionIdAttribute { ConnectionId = connectionId }),
                TurnAttributeMapper.Encode(new TurnXorPeerAddressAttribute { EndPoint = peerEndPoint }, txId)
            ]
        };

        return _codec.Encode(indication);
    }
}
