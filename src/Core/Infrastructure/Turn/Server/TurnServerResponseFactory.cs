using CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Builds TURN/STUN responses with consistent error/auth attribute handling.
/// </summary>
internal sealed class TurnServerResponseFactory
{
    private readonly TurnAuthOptions? _authOptions;

    /// <summary>
    /// Creates a response factory bound to optional TURN auth settings.
    /// </summary>
    public TurnServerResponseFactory(TurnAuthOptions? authOptions)
    {
        _authOptions = authOptions;
    }

    /// <summary>
    /// Creates a 401 Unauthorized response.
    /// </summary>
    public StunMessage BuildUnauthorizedResponse(StunMessage request, string realm)
        => BuildErrorResponse(request, 401, "Unauthorized", includeAuthAttributes: true, realmOverride: realm);

    /// <summary>
    /// Creates a 438 Stale Nonce response.
    /// </summary>
    public StunMessage BuildStaleNonceResponse(StunMessage request, string realm)
        => BuildErrorResponse(request, 438, "Stale Nonce", includeAuthAttributes: true, realmOverride: realm);

    /// <summary>
    /// Creates a generic TURN/STUN error response.
    /// </summary>
    public StunMessage BuildErrorResponse(
        StunMessage request,
        int code,
        string reason,
        bool includeAuthAttributes,
        string? realmOverride = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        var attrs = new List<StunAttribute>();
        if (includeAuthAttributes && _authOptions is not null)
        {
            var realm = string.IsNullOrWhiteSpace(realmOverride) ? _authOptions.Realm : realmOverride;
            attrs.Add(new RealmAttribute { Value = realm });
            attrs.Add(new NonceAttribute { Value = _authOptions.NonceManager.GenerateNonce() });
        }

        attrs.Add(new ErrorCodeAttribute { Code = code, Reason = reason });

        return new StunMessage
        {
            MessageClass = StunMessageClass.ErrorResponse,
            MessageMethod = request.MessageMethod,
            TransactionId = request.TransactionId,
            Attributes = attrs
        };
    }

    /// <summary>
    /// Creates a success response carrying optional attributes.
    /// </summary>
    public StunMessage BuildSuccessResponse(StunMessage request, IReadOnlyList<StunAttribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(attributes);

        return new StunMessage
        {
            MessageClass = StunMessageClass.SuccessResponse,
            MessageMethod = request.MessageMethod,
            TransactionId = request.TransactionId,
            Attributes = attributes
        };
    }
}
