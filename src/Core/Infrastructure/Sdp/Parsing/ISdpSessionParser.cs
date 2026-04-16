using CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Parsing;

/// <summary>
/// Parses raw SDP text into structured session models.
/// </summary>
internal interface ISdpSessionParser
{
    /// <summary>
    /// Parses SDP text. Throws on malformed mandatory lines.
    /// </summary>
    SdpSessionDescription Parse(string sdp);
}

