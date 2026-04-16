namespace CalloraVoipSdk.Core.Security;

public enum SrtpPolicy
{
    /// <summary>No SRTP. Media is unencrypted.</summary>
    Disabled,

    /// <summary>Use SRTP if the remote party offers it, otherwise fall back to plain RTP.</summary>
    Optional,

    /// <summary>Require SRTP. Calls without SRTP will fail.</summary>
    Required
}
