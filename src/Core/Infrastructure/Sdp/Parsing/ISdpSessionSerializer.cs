using CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Parsing;

/// <summary>
/// Serializes SDP models to wire text.
/// </summary>
internal interface ISdpSessionSerializer
{
    /// <summary>
    /// Serializes one SDP session model to CRLF-separated text.
    /// </summary>
    string Serialize(SdpSessionDescription session);
}

