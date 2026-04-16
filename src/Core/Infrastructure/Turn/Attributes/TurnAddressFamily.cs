namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN address-family codes used in REQUESTED-ADDRESS-FAMILY and XOR address attributes.
/// </summary>
internal enum TurnAddressFamily : byte
{
    /// <summary>IPv4 address family code (0x01).</summary>
    IPv4 = 0x01,

    /// <summary>IPv6 address family code (0x02).</summary>
    IPv6 = 0x02
}
