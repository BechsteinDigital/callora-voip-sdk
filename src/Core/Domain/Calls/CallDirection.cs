namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Indicates whether a call was initiated by the local party or received from a remote party.
/// </summary>
public enum CallDirection
{
    /// <summary>The call was received from a remote party.</summary>
    Inbound,

    /// <summary>The call was initiated by the local party.</summary>
    Outbound
}
