namespace CalloraVoipSdk;

/// <summary>
/// Classifies one ICE helper server entry.
/// </summary>
public enum IceServerType
{
    /// <summary>
    /// STUN server used for server-reflexive candidate discovery.
    /// </summary>
    Stun,

    /// <summary>
    /// TURN server used for relay candidate workflows.
    /// </summary>
    Turn
}
