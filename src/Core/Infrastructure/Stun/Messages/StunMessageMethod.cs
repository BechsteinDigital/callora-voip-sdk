namespace CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

/// <summary>
/// STUN message methods (RFC 5389 §6).
/// Extended by TURN (RFC 5766) with Allocate/Refresh/Send/Data/CreatePermission/ChannelBind.
/// </summary>
internal enum StunMessageMethod : ushort
{
    /// <summary>Binding method — used to discover the public endpoint via NAT traversal (RFC 5389).</summary>
    Binding = 0x0001
}
