using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Manages RFC 6062 pending TCP peer connections and bound data relays.
/// </summary>
internal sealed class TurnTcpConnectionBroker : IAsyncDisposable
{
    private readonly ConcurrentDictionary<uint, TurnTcpPendingConnection> _pendingById = new();
    private readonly ConcurrentDictionary<Guid, TurnTcpPendingConnection> _boundByClientStream = new();
    private int _nextConnectionId = RandomNumberGenerator.GetInt32(1, int.MaxValue);
    private static readonly TimeSpan PendingTtl = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Creates an outgoing TCP peer connection and registers a new CONNECTION-ID.
    /// </summary>
    public async Task<uint> ConnectPeerAsync(string allocationKey, IPEndPoint peerEndPoint, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allocationKey);
        ArgumentNullException.ThrowIfNull(peerEndPoint);

        var peerClient = new TcpClient(peerEndPoint.AddressFamily)
        {
            NoDelay = true
        };

        try
        {
            await peerClient.ConnectAsync(peerEndPoint.Address, peerEndPoint.Port, ct).ConfigureAwait(false);
        }
        catch
        {
            peerClient.Dispose();
            throw;
        }

        return RegisterIncomingPeer(allocationKey, peerEndPoint, peerClient);
    }

    /// <summary>
    /// Registers an accepted incoming peer TCP connection and returns a CONNECTION-ID.
    /// </summary>
    public uint RegisterIncomingPeer(string allocationKey, IPEndPoint peerEndPoint, TcpClient peerClient)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allocationKey);
        ArgumentNullException.ThrowIfNull(peerEndPoint);
        ArgumentNullException.ThrowIfNull(peerClient);

        var connectionId = NextConnectionId();
        var pending = new TurnTcpPendingConnection
        {
            ConnectionId = connectionId,
            AllocationKey = allocationKey,
            PeerEndPoint = peerEndPoint,
            PeerClient = peerClient,
            PeerStream = peerClient.GetStream(),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _pendingById[connectionId] = pending;
        PurgeExpiredPending(DateTimeOffset.UtcNow);
        return connectionId;
    }

    /// <summary>
    /// Binds a CONNECTION-ID to a client stream ID.
    /// </summary>
    public bool TryBind(uint connectionId, Guid clientStreamId, out string allocationKey)
    {
        allocationKey = string.Empty;

        if (!_pendingById.TryRemove(connectionId, out var pending))
            return false;

        allocationKey = pending.AllocationKey;

        if (_boundByClientStream.TryGetValue(clientStreamId, out var existing))
            _ = existing.DisposeAsync();

        _boundByClientStream[clientStreamId] = pending;
        return true;
    }

    /// <summary>
    /// Runs raw TCP relay when a stream has been bound via CONNECTION-BIND.
    /// </summary>
    public async Task<bool> TryRunBoundRelayAsync(
        TurnStreamConnection clientConnection,
        ILogger logger,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(clientConnection);
        ArgumentNullException.ThrowIfNull(logger);

        if (!_boundByClientStream.TryRemove(clientConnection.Id, out var pending))
            return false;

        await using (pending.ConfigureAwait(false))
        using (var linked = CancellationTokenSource.CreateLinkedTokenSource(ct))
        {
            var relayCt = linked.Token;

            var clientToPeer = PipeAsync(clientConnection.Stream, pending.PeerStream, relayCt);
            var peerToClient = PipeAsync(pending.PeerStream, clientConnection.Stream, relayCt);

            Task completed;
            try
            {
                completed = await Task.WhenAny(clientToPeer, peerToClient).ConfigureAwait(false);
                await completed.ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (relayCt.IsCancellationRequested)
            {
                return true;
            }
            catch (Exception ex)
            {
                logger.LogDebug(
                    ex,
                    "TURN TCP relay terminated with error for stream {StreamId}",
                    clientConnection.Id);
            }
            finally
            {
                linked.Cancel();
            }

            try
            {
                await Task.WhenAll(clientToPeer, peerToClient).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                logger.LogDebug(
                    ex,
                    "TURN TCP relay shutdown encountered stream errors for stream {StreamId}",
                    clientConnection.Id);
            }
        }

        return true;
    }

    /// <summary>
    /// Removes and disposes all pending/bound connections belonging to an allocation.
    /// </summary>
    public void RemoveByAllocation(string allocationKey)
    {
        if (string.IsNullOrWhiteSpace(allocationKey))
            return;

        foreach (var (connectionId, pending) in _pendingById)
        {
            if (!string.Equals(pending.AllocationKey, allocationKey, StringComparison.Ordinal))
                continue;

            if (_pendingById.TryRemove(connectionId, out var removed))
                _ = removed.DisposeAsync();
        }

        foreach (var (streamId, pending) in _boundByClientStream)
        {
            if (!string.Equals(pending.AllocationKey, allocationKey, StringComparison.Ordinal))
                continue;

            if (_boundByClientStream.TryRemove(streamId, out var removed))
                _ = removed.DisposeAsync();
        }
    }

    /// <summary>
    /// Removes and disposes one bound connection by client stream.
    /// </summary>
    public void RemoveByClientStream(Guid clientStreamId)
    {
        if (_boundByClientStream.TryRemove(clientStreamId, out var pending))
            _ = pending.DisposeAsync();
    }

    /// <summary>
    /// Removes and disposes one pending connection by CONNECTION-ID.
    /// </summary>
    public void RemovePending(uint connectionId)
    {
        if (_pendingById.TryRemove(connectionId, out var pending))
            _ = pending.DisposeAsync();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        foreach (var (_, pending) in _pendingById)
            _ = pending.DisposeAsync();
        _pendingById.Clear();

        foreach (var (_, pending) in _boundByClientStream)
            _ = pending.DisposeAsync();
        _boundByClientStream.Clear();

        return ValueTask.CompletedTask;
    }

    private static async Task PipeAsync(Stream source, Stream destination, CancellationToken ct)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        try
        {
            while (!ct.IsCancellationRequested)
            {
                int read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false);
                if (read == 0)
                    break;

                await destination.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private uint NextConnectionId()
    {
        int value = Interlocked.Increment(ref _nextConnectionId);
        if (value <= 0)
            value = Interlocked.Exchange(ref _nextConnectionId, 1);

        return unchecked((uint)value);
    }

    private void PurgeExpiredPending(DateTimeOffset nowUtc)
    {
        foreach (var (connectionId, pending) in _pendingById)
        {
            if (nowUtc - pending.CreatedAtUtc <= PendingTtl)
                continue;

            if (_pendingById.TryRemove(connectionId, out var removed))
                _ = removed.DisposeAsync();
        }
    }
}
