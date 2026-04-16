namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Default UAS user identity policy that accepts every inbound Request-URI.
///
/// Used when no explicit policy is registered via dependency injection.
/// Suitable for single-user SDKs, softphones, or deployments where the SIP proxy
/// is responsible for user-address validation before forwarding.
/// </summary>
internal sealed class AcceptAllSipUasUserIdentityPolicy : ISipUasUserIdentityPolicy
{
    /// <summary>
    /// Shared singleton instance.
    /// </summary>
    public static readonly ISipUasUserIdentityPolicy Instance = new AcceptAllSipUasUserIdentityPolicy();

    private AcceptAllSipUasUserIdentityPolicy() { }

    /// <inheritdoc />
    public bool IsServedUser(string requestUri) => true;
}
