namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Unified result model for extended call control operations.
/// </summary>
public sealed class CallActionResult
{
    /// <summary>
    /// Operation status.
    /// </summary>
    public required CallActionStatus Status { get; init; }

    /// <summary>
    /// Optional SIP status code associated with the result.
    /// </summary>
    public int? SipStatusCode { get; init; }

    /// <summary>
    /// Human-readable reason for the result.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// True when <see cref="Status"/> is <see cref="CallActionStatus.Succeeded"/>.
    /// </summary>
    public bool IsSuccess => Status == CallActionStatus.Succeeded;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CallActionResult Success(
        string? reason = null,
        int? sipStatusCode = null) =>
        new()
        {
            Status = CallActionStatus.Succeeded,
            Reason = reason,
            SipStatusCode = sipStatusCode
        };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static CallActionResult Failure(
        CallActionStatus status,
        string reason,
        int? sipStatusCode = null)
    {
        if (status == CallActionStatus.Succeeded)
            throw new ArgumentException("Use Success for successful outcomes.", nameof(status));

        return new CallActionResult
        {
            Status = status,
            Reason = reason,
            SipStatusCode = sipStatusCode
        };
    }
}
