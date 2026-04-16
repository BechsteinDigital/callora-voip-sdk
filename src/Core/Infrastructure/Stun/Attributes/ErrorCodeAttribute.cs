namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN ERROR-CODE attribute (RFC 5389 §15.6).
/// Carried in error responses to indicate the reason a request was rejected.
/// </summary>
internal sealed class ErrorCodeAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.ErrorCode;

    /// <summary>
    /// Numeric three-digit error code (e.g. 400 Bad Request, 401 Unauthorized,
    /// 420 Unknown Attribute, 438 Stale Nonce, 500 Server Error).
    /// </summary>
    public int Code { get; init; }

    /// <summary>Human-readable reason phrase (UTF-8, not null-terminated).</summary>
    public string Reason { get; init; } = string.Empty;
}
