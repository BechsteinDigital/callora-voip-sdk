namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event payload for one inbound DTMF indication received via SIP INFO.
/// </summary>
internal sealed class SipDtmfReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a DTMF event payload.
    /// </summary>
    public SipDtmfReceivedEventArgs(byte toneCode, int durationMilliseconds)
    {
        ToneCode = toneCode;
        DurationMilliseconds = durationMilliseconds;
    }

    /// <summary>
    /// DTMF tone code mapped to domain tone values (0-15).
    /// </summary>
    public byte ToneCode { get; }

    /// <summary>
    /// Tone duration in milliseconds when provided by sender.
    /// </summary>
    public int DurationMilliseconds { get; }
}

