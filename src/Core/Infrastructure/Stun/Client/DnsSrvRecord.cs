namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Represents a single DNS SRV resource record (RFC 2782).
/// SRV records encode service endpoints with priority, weight, port and target host.
/// </summary>
/// <param name="Priority">
/// Lower values have higher priority. Clients MUST attempt the lowest priority first.
/// </param>
/// <param name="Weight">
/// Used for weighted random selection among equal-priority records.
/// Higher weight = more likely to be selected.
/// </param>
/// <param name="Port">TCP or UDP port of the service.</param>
/// <param name="Target">Fully qualified domain name of the target host.</param>
internal sealed record DnsSrvRecord(ushort Priority, ushort Weight, ushort Port, string Target);
