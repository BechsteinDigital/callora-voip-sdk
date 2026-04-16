using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// In-memory store for RFC 8016 mobility tickets.
/// </summary>
internal sealed class TurnMobilityTicketStore
{
    private readonly ConcurrentDictionary<string, TurnMobilityTicketEntry> _entries = new(StringComparer.Ordinal);
    private static readonly TimeSpan PurgeInterval = TimeSpan.FromSeconds(30);
    private long _nextPurgeAtUtcTicks = DateTimeOffset.UtcNow.Add(PurgeInterval).UtcTicks;

    /// <summary>
    /// Issues a fresh opaque mobility ticket for an allocation key.
    /// </summary>
    public byte[] Issue(string allocationKey, DateTimeOffset expiresAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allocationKey);

        var now = DateTimeOffset.UtcNow;
        var ticket = RandomNumberGenerator.GetBytes(24);
        var key = ToKey(ticket);

        _entries[key] = new TurnMobilityTicketEntry
        {
            AllocationKey = allocationKey,
            ExpiresAtUtc = expiresAtUtc
        };

        TryPurgeExpired(now);
        return ticket;
    }

    /// <summary>
    /// Resolves an allocation key from a ticket when still valid.
    /// </summary>
    public bool TryResolve(ReadOnlySpan<byte> ticket, out string allocationKey)
    {
        allocationKey = string.Empty;

        if (ticket.IsEmpty)
            return false;

        var key = ToKey(ticket);
        if (!_entries.TryGetValue(key, out var entry))
            return false;

        if (DateTimeOffset.UtcNow > entry.ExpiresAtUtc)
        {
            _entries.TryRemove(key, out _);
            return false;
        }

        allocationKey = entry.AllocationKey;
        return true;
    }

    /// <summary>
    /// Removes a specific ticket.
    /// </summary>
    public void Remove(ReadOnlySpan<byte> ticket)
    {
        if (ticket.IsEmpty)
            return;

        _entries.TryRemove(ToKey(ticket), out _);
    }

    /// <summary>
    /// Removes all tickets associated with a specific allocation key.
    /// </summary>
    public void RemoveByAllocation(string allocationKey)
    {
        if (string.IsNullOrWhiteSpace(allocationKey))
            return;

        foreach (var (ticket, entry) in _entries)
        {
            if (string.Equals(entry.AllocationKey, allocationKey, StringComparison.Ordinal))
                _entries.TryRemove(ticket, out _);
        }
    }

    private void TryPurgeExpired(DateTimeOffset nowUtc)
    {
        var dueTicks = Volatile.Read(ref _nextPurgeAtUtcTicks);
        if (nowUtc.UtcTicks < dueTicks)
            return;

        var nextDue = nowUtc.Add(PurgeInterval).UtcTicks;
        if (Interlocked.CompareExchange(ref _nextPurgeAtUtcTicks, nextDue, dueTicks) != dueTicks)
            return;

        PurgeExpired(nowUtc);
    }

    private void PurgeExpired(DateTimeOffset nowUtc)
    {
        foreach (var (ticket, entry) in _entries)
        {
            if (nowUtc > entry.ExpiresAtUtc)
                _entries.TryRemove(ticket, out _);
        }
    }

    private static string ToKey(ReadOnlySpan<byte> ticket) => Convert.ToHexString(ticket);
}
