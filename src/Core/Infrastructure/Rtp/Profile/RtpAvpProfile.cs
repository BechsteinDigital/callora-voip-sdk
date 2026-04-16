namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Profile;

/// <summary>
/// Well-known static payload type assignments for the RTP Audio/Video Profile (RFC 3551 §6).
/// Payload types 96–127 are dynamic and assigned via SDP negotiation (RFC 3551 §3).
/// </summary>
internal static class RtpAvpProfile
{
    // -------------------------------------------------------------------------
    // Audio codecs — static payload types (RFC 3551 §6)
    // -------------------------------------------------------------------------

    /// <summary>PCMU — G.711 µ-law, 8000 Hz, mono (RFC 3551 §4.5.14).</summary>
    public const byte Pcmu = 0;

    /// <summary>GSM — GSM Full Rate 13 kbps, 8000 Hz, mono (RFC 3551 §4.5.8).</summary>
    public const byte Gsm = 3;

    /// <summary>G723 — G.723.1, 8000 Hz, mono (RFC 3551 §4.5.3).</summary>
    public const byte G723 = 4;

    /// <summary>DVI4 — ADPCM, 8000 Hz, mono (RFC 3551 §4.5.2).</summary>
    public const byte Dvi4_8000 = 5;

    /// <summary>DVI4 — ADPCM, 16000 Hz, mono (RFC 3551 §4.5.2).</summary>
    public const byte Dvi4_16000 = 6;

    /// <summary>LPC — LPC, 8000 Hz, mono (RFC 3551 §4.5.12).</summary>
    public const byte Lpc = 7;

    /// <summary>PCMA — G.711 A-law, 8000 Hz, mono (RFC 3551 §4.5.14).</summary>
    public const byte Pcma = 8;

    /// <summary>G722 — G.722 wideband, 8000 Hz clock (16 kHz audio), mono (RFC 3551 §4.5.2).</summary>
    public const byte G722 = 9;

    /// <summary>L16 stereo — 16-bit linear PCM, 44100 Hz, stereo (RFC 3551 §4.5.11).</summary>
    public const byte L16Stereo = 10;

    /// <summary>L16 mono — 16-bit linear PCM, 44100 Hz, mono (RFC 3551 §4.5.11).</summary>
    public const byte L16Mono = 11;

    /// <summary>QCELP — QCELP, 8000 Hz, mono (RFC 3551 §4.5.15).</summary>
    public const byte Qcelp = 12;

    /// <summary>CN — Comfort Noise (RFC 3389), 8000 Hz (RFC 3551 §4.1).</summary>
    public const byte Cn = 13;

    /// <summary>MPA — MPEG-1/2 audio (RFC 2250), 90000 Hz clock (RFC 3551 §4.5.13).</summary>
    public const byte Mpa = 14;

    /// <summary>G728 — G.728 LD-CELP, 8000 Hz, mono (RFC 3551 §4.5.4).</summary>
    public const byte G728 = 15;

    /// <summary>DVI4 — ADPCM, 11025 Hz, mono (RFC 3551 §4.5.2).</summary>
    public const byte Dvi4_11025 = 16;

    /// <summary>DVI4 — ADPCM, 22050 Hz, mono (RFC 3551 §4.5.2).</summary>
    public const byte Dvi4_22050 = 17;

    /// <summary>G729 — G.729 CS-ACELP 8 kbps, 8000 Hz, mono (RFC 3551 §4.5.6).</summary>
    public const byte G729 = 18;

    // -------------------------------------------------------------------------
    // Video codecs — static payload types (RFC 3551 §6)
    // -------------------------------------------------------------------------

    /// <summary>CelB — Sun CellB video, 90000 Hz clock (RFC 3551 §6.1).</summary>
    public const byte CelB = 25;

    /// <summary>JPEG — JPEG video, 90000 Hz clock (RFC 2435, RFC 3551 §6.1).</summary>
    public const byte Jpeg = 26;

    /// <summary>nv — nv video, 90000 Hz clock (RFC 3551 §6.1).</summary>
    public const byte Nv = 28;

    /// <summary>H261 — H.261 video, 90000 Hz clock (RFC 4587, RFC 3551 §6.1).</summary>
    public const byte H261 = 31;

    /// <summary>MPV — MPEG-1/2 video, 90000 Hz clock (RFC 2250, RFC 3551 §6.1).</summary>
    public const byte Mpv = 32;

    /// <summary>MP2T — MPEG-2 transport stream, 90000 Hz clock (RFC 2250, RFC 3551 §6.1).</summary>
    public const byte Mp2T = 33;

    /// <summary>H263 — H.263 video, 90000 Hz clock (RFC 4629, RFC 3551 §6.1).</summary>
    public const byte H263 = 34;

    // -------------------------------------------------------------------------
    // Dynamic range
    // -------------------------------------------------------------------------

    /// <summary>Lowest dynamic payload type (RFC 3551 §3). Assigned via SDP negotiation.</summary>
    public const byte DynamicMin = 96;

    /// <summary>Highest dynamic payload type (RFC 3551 §3). Assigned via SDP negotiation.</summary>
    public const byte DynamicMax = 127;

    /// <summary>Returns true when <paramref name="payloadType"/> is in the dynamic range 96–127.</summary>
    public static bool IsDynamic(byte payloadType) => payloadType is >= DynamicMin and <= DynamicMax;

    // -------------------------------------------------------------------------
    // Clock rates for static payload types (RFC 3551 §6)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the clock rate in Hz for a static payload type, or null for dynamic types.
    /// The clock rate determines RTP timestamp increments per second.
    /// </summary>
    public static int? ClockRateFor(byte payloadType) => payloadType switch
    {
        Pcmu       => 8_000,
        Gsm        => 8_000,
        G723       => 8_000,
        Dvi4_8000  => 8_000,
        Dvi4_16000 => 16_000,
        Lpc        => 8_000,
        Pcma       => 8_000,
        G722       => 8_000,
        L16Stereo  => 44_100,
        L16Mono    => 44_100,
        Qcelp      => 8_000,
        Cn         => 8_000,
        Mpa        => 90_000,
        G728       => 8_000,
        Dvi4_11025 => 11_025,
        Dvi4_22050 => 22_050,
        G729       => 8_000,
        CelB       => 90_000,
        Jpeg       => 90_000,
        Nv         => 90_000,
        H261       => 90_000,
        Mpv        => 90_000,
        Mp2T       => 90_000,
        H263       => 90_000,
        _          => null
    };
}
