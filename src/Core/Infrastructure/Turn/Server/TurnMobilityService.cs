using System.Collections.Concurrent;
using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// RFC 8016 mobility helper around ticket issuance, lookup, and allocation migration.
/// </summary>
internal sealed class TurnMobilityService
{
    private readonly TurnMobilityTicketStore _tickets = new();

    /// <summary>
    /// Issues a fresh ticket for the provided allocation.
    /// </summary>
    public byte[] IssueTicket(TurnServerAllocation allocation)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        return _tickets.Issue(allocation.ClientKey, allocation.ExpiresAtUtc);
    }

    /// <summary>
    /// Resolves a ticket to the current allocation, if available.
    /// </summary>
    public bool TryResolveAllocationByTicket(
        ReadOnlyMemory<byte> ticket,
        ConcurrentDictionary<string, TurnServerAllocation> allocationsByClient,
        out string allocationKey,
        out TurnServerAllocation? allocation)
    {
        ArgumentNullException.ThrowIfNull(allocationsByClient);

        allocationKey = string.Empty;
        allocation = null;

        if (!_tickets.TryResolve(ticket.Span, out allocationKey))
            return false;

        return allocationsByClient.TryGetValue(allocationKey, out allocation);
    }

    /// <summary>
    /// Migrates an allocation from an old client key to a new client tuple.
    /// </summary>
    public bool TryMigrateAllocationToClient(
        ConcurrentDictionary<string, TurnServerAllocation> allocationsByClient,
        string oldClientKey,
        TurnClientContext newContext,
        out TurnServerAllocation? allocation)
    {
        ArgumentNullException.ThrowIfNull(allocationsByClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(oldClientKey);

        allocation = null;

        if (!allocationsByClient.TryGetValue(oldClientKey, out var existing))
            return false;

        existing.ClientKey = newContext.ClientKey;
        if (newContext.Transport == TurnServerTransport.Udp)
            existing.ClientUdpEndPoint = newContext.RemoteEndPoint;

        allocationsByClient[newContext.ClientKey] = existing;
        if (!string.Equals(oldClientKey, newContext.ClientKey, StringComparison.Ordinal))
            allocationsByClient.TryRemove(oldClientKey, out _);

        allocation = existing;
        return true;
    }

    /// <summary>
    /// Removes a specific issued ticket.
    /// </summary>
    public void RemoveTicket(ReadOnlySpan<byte> ticket)
        => _tickets.Remove(ticket);

    /// <summary>
    /// Removes all tickets for one allocation key.
    /// </summary>
    public void RemoveTicketsForAllocation(string allocationKey)
        => _tickets.RemoveByAllocation(allocationKey);

    /// <summary>
    /// True when the TURN address family is one of the RFC-defined families.
    /// </summary>
    public static bool IsKnownAddressFamily(TurnAddressFamily family)
        => family is TurnAddressFamily.IPv4 or TurnAddressFamily.IPv6;

    /// <summary>
    /// True when peer and allocation use the same address family.
    /// </summary>
    public static bool IsPeerFamilyMatchingAllocation(TurnServerAllocation allocation, IPEndPoint peerEndPoint)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        ArgumentNullException.ThrowIfNull(peerEndPoint);
        return allocation.RelayedEndPoint.AddressFamily == peerEndPoint.AddressFamily;
    }
}
