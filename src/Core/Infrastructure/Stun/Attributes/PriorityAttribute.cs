namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// ICE PRIORITY attribute (RFC 8445 §16.1).
/// Comprehension-required attribute sent in ICE connectivity check Binding Requests.
/// Carries the priority value of the candidate pair being checked.
/// The server uses this value if it creates a peer-reflexive candidate from the request.
/// Wire format: 4-byte unsigned integer, big-endian.
/// </summary>
internal sealed class PriorityAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Priority;

    /// <summary>
    /// The 32-bit candidate priority as defined in RFC 8445 §5.1.2.
    /// Higher values indicate higher priority.
    /// </summary>
    public required uint Value { get; init; }
}
