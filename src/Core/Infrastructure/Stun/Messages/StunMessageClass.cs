namespace CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

/// <summary>
/// STUN message class encoded in bits [8] and [4] of the 14-bit message type field (RFC 5389 §6).
/// Values represent the combined bit pattern of C1 (bit 8) and C0 (bit 4) as they appear in the
/// message type word, making it possible to extract the class with a simple bitwise AND of 0x0110.
/// </summary>
internal enum StunMessageClass : ushort
{
    /// <summary>Client-to-server request expecting a response.</summary>
    Request = 0x0000,

    /// <summary>One-way message; no response is expected.</summary>
    Indication = 0x0010,

    /// <summary>Successful response to a request (2xx equivalent).</summary>
    SuccessResponse = 0x0100,

    /// <summary>Error response to a request.</summary>
    ErrorResponse = 0x0110
}
