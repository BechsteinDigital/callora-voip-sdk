namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// TURN method codes carried in the STUN type field (RFC 8656, RFC 6062).
/// </summary>
internal enum TurnMessageMethod : ushort
{
    /// <summary>Allocate transaction method.</summary>
    Allocate = 0x0003,

    /// <summary>Refresh transaction method.</summary>
    Refresh = 0x0004,

    /// <summary>Send indication method.</summary>
    Send = 0x0006,

    /// <summary>Data indication method.</summary>
    Data = 0x0007,

    /// <summary>CreatePermission transaction method.</summary>
    CreatePermission = 0x0008,

    /// <summary>ChannelBind transaction method.</summary>
    ChannelBind = 0x0009,

    /// <summary>RFC 6062 Connect transaction method.</summary>
    Connect = 0x000A,

    /// <summary>RFC 6062 ConnectionBind transaction method.</summary>
    ConnectionBind = 0x000B,

    /// <summary>RFC 6062 ConnectionAttempt indication method.</summary>
    ConnectionAttempt = 0x000C
}
