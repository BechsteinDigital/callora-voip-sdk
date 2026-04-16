namespace CalloraVoipSdk.Core.Infrastructure.Sip.Authentication;

/// <summary>
/// Contract for SIP Digest Authorization header generation.
/// </summary>
internal interface ISipDigestAuthenticator
{
    /// <summary>
    /// Attempts to build an Authorization header from a Digest challenge.
    /// </summary>
    bool TryCreateAuthorizationHeader(
        string? challengeHeader,
        string username,
        string password,
        string method,
        string requestUri,
        int nonceCount,
        out string authorizationHeader);
}
