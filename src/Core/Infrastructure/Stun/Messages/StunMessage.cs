using System.Security.Cryptography;
using CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Stun.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

/// <summary>
/// Immutable STUN message model used for both sending and receiving.
/// Carries the message class, method, transaction ID, and an ordered attribute list.
/// All wire serialisation is delegated to <see cref="StunMessageCodec"/>.
/// </summary>
internal sealed class StunMessage
{
    /// <summary>Message class: request, indication, or response (success/error).</summary>
    public required StunMessageClass MessageClass { get; init; }

    /// <summary>Message method identifying the STUN operation (e.g. Binding).</summary>
    public required StunMessageMethod MessageMethod { get; init; }

    /// <summary>
    /// 12-byte transaction ID that correlates requests with their responses.
    /// Must contain exactly <see cref="StunWireConstants.TransactionIdLength"/> bytes.
    /// </summary>
    public required byte[] TransactionId { get; init; }

    /// <summary>Ordered list of STUN attributes attached to this message.</summary>
    public IReadOnlyList<StunAttribute> Attributes { get; init; } = [];

    /// <summary>
    /// Creates a new Binding Request with a cryptographically random transaction ID.
    /// </summary>
    public static StunMessage CreateBindingRequest()
    {
        var txId = new byte[StunWireConstants.TransactionIdLength];
        RandomNumberGenerator.Fill(txId);
        return new StunMessage
        {
            MessageClass  = StunMessageClass.Request,
            MessageMethod = StunMessageMethod.Binding,
            TransactionId = txId
        };
    }

    /// <summary>
    /// Creates a Binding Success Response correlated with the given request transaction ID.
    /// </summary>
    /// <param name="requestTransactionId">Transaction ID copied from the incoming request.</param>
    /// <param name="attributes">Attributes to include (typically XOR-MAPPED-ADDRESS).</param>
    public static StunMessage CreateBindingResponse(
        byte[] requestTransactionId,
        IReadOnlyList<StunAttribute> attributes)
    {
        return new StunMessage
        {
            MessageClass  = StunMessageClass.SuccessResponse,
            MessageMethod = StunMessageMethod.Binding,
            TransactionId = requestTransactionId,
            Attributes    = attributes
        };
    }
}
