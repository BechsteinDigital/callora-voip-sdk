namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Internal exception used for TURN 401/438 challenge handling.
/// </summary>
internal sealed class TurnChallengeException : TurnException
{
    /// <summary>TURN/STUN error code.</summary>
    public int ErrorCode { get; }

    /// <summary>Realm carried by the challenge, when present.</summary>
    public string? Realm { get; }

    /// <summary>Nonce carried by the challenge, when present.</summary>
    public string? Nonce { get; }

    /// <summary>
    /// Creates a challenge exception.
    /// </summary>
    public TurnChallengeException(int errorCode, string reason, string? realm, string? nonce)
        : base($"TURN challenge {errorCode}: {reason}")
    {
        ErrorCode = errorCode;
        Realm = realm;
        Nonce = nonce;
    }
}
