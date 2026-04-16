namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// Known STUN attribute type codes (RFC 5389 §15).
/// Comprehension-required range: 0x0000–0x7FFF.
/// Comprehension-optional range: 0x8000–0xFFFF.
/// Unknown attribute types are stored as <see cref="UnknownRawAttribute"/>.
/// </summary>
internal enum StunAttributeType : ushort
{
    /// <summary>Sentinel for unknown/unrecognised attribute types preserved as raw bytes.</summary>
    Unknown = 0x0000,

    /// <summary>Public endpoint of the client (deprecated — prefer XorMappedAddress).</summary>
    MappedAddress = 0x0001,

    /// <summary>RFC 3489 legacy attribute requesting response from alternate IP/port. Rejected with 400 by RFC 5389 servers (§12).</summary>
    ChangeRequest = 0x0003,

    /// <summary>UTF-8 username credential for authentication.</summary>
    Username = 0x0006,

    /// <summary>HMAC-SHA1 over the STUN message for integrity protection.</summary>
    MessageIntegrity = 0x0008,

    /// <summary>Error code and reason phrase in error responses.</summary>
    ErrorCode = 0x0009,

    /// <summary>List of attribute type codes not understood by the receiver.</summary>
    UnknownAttributes = 0x000A,

    /// <summary>Authentication realm for long-term credential mechanism.</summary>
    Realm = 0x0014,

    /// <summary>Nonce value for replay protection in long-term credential mechanism.</summary>
    Nonce = 0x0015,

    /// <summary>
    /// OAuth-style third-party access token (RFC 7635 §6.2).
    /// This is a comprehension-required attribute.
    /// </summary>
    AccessToken = 0x001B,

    /// <summary>XOR-obfuscated mapped public endpoint (preferred form, RFC 5389 §15.2).</summary>
    XorMappedAddress = 0x0020,

    /// <summary>Candidate priority for ICE connectivity checks (RFC 8445 §16.1).</summary>
    Priority = 0x0024,

    /// <summary>Flag indicating the controlling agent nominates this candidate pair (RFC 8445 §16.1).</summary>
    UseCandidate = 0x0025,

    /// <summary>Human-readable software description string (comprehension-optional).</summary>
    Software = 0x8022,

    /// <summary>Alternate server endpoint for redirection responses (comprehension-optional).</summary>
    AlternateServer = 0x8023,

    /// <summary>CRC32 fingerprint XOR'd with 0x5354554E for protocol multiplexing (comprehension-optional).</summary>
    Fingerprint = 0x8028,

    /// <summary>64-bit tiebreaker for the ICE controlled agent in role conflict resolution (RFC 8445 §16.1).</summary>
    IceControlled = 0x8029,

    /// <summary>64-bit tiebreaker for the ICE controlling agent in role conflict resolution (RFC 8445 §16.1).</summary>
    IceControlling = 0x802A,

    /// <summary>
    /// Indicates that the server supports third-party authorization and carries the STUN server name
    /// to be presented to the authorization server (RFC 7635 §6.1).
    /// This is a comprehension-optional attribute.
    /// </summary>
    ThirdPartyAuthorization = 0x802E
}
