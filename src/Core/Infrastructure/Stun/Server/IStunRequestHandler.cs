using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Stateless processor for incoming STUN requests.
/// Receives a decoded message, the original raw bytes (needed for integrity verification),
/// and the sender endpoint. Returns a response payload to send back, or null to drop silently.
/// <para>
/// Wire encoding of the response (including MESSAGE-INTEGRITY and FINGERPRINT) is handled
/// by the <see cref="StunServer"/> transport layer, not by the handler.
/// </para>
/// </summary>
internal interface IStunRequestHandler
{
    /// <summary>
    /// Processes an incoming STUN request and produces a response.
    /// </summary>
    /// <param name="request">The decoded request message.</param>
    /// <param name="rawRequest">The complete raw bytes as received (used for integrity verification).</param>
    /// <param name="sender">The endpoint from which the request was received.</param>
    /// <returns>The response payload to send, or null to drop the packet silently.</returns>
    StunRequestHandlingResult? Handle(StunMessage request, ReadOnlySpan<byte> rawRequest, IPEndPoint sender);
}
