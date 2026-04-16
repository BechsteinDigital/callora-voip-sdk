using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Result of STUN request handling, including the response message and an optional
/// per-response integrity key override.
/// </summary>
internal sealed class StunRequestHandlingResult
{
    /// <summary>
    /// Response message to send on the wire.
    /// </summary>
    public required StunMessage Response { get; init; }

    /// <summary>
    /// Optional HMAC key used to encode MESSAGE-INTEGRITY for this specific response.
    /// When null, the server-level default response key is used (if configured).
    /// </summary>
    public byte[]? ResponseIntegrityKey { get; init; }
}
