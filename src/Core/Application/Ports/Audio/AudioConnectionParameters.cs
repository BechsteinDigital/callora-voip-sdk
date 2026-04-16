using System.Collections.ObjectModel;

namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Codec and stream parameters passed to <see cref="IAudioDevice.Connect"/> so the
/// device can open hardware streams at the correct sample rate and apply the right codec.
/// </summary>
public sealed class AudioConnectionParameters
{
    /// <summary>Sensible defaults for PCMU (8 kHz, PT=0).</summary>
    public static readonly AudioConnectionParameters Default = new();

    /// <summary>RTP payload type (0 = PCMU, 8 = PCMA, 9 = G.722).</summary>
    public int PayloadType { get; init; } = 0;

    /// <summary>Normalized negotiated codec name (for example PCMU, PCMA, G722).</summary>
    public string CodecName { get; init; } = "PCMU";

    /// <summary>
    /// Payload-type to codec-name mapping from SDP for this call leg.
    /// Enables correct decode of dynamic RTP payload types.
    /// </summary>
    public IReadOnlyDictionary<int, string> PayloadTypeCodecMap { get; init; }
        = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>());

    /// <summary>RTP clock rate in Hz (e.g. 8000 for G.711 and G.722).</summary>
    public int ClockRate { get; init; } = 8000;

    /// <summary>
    /// Audio hardware sample rate in Hz.
    /// For G.711 this matches <see cref="ClockRate"/> (8000 Hz).
    /// For G.722 this is 16000 Hz even though the RTP clock is 8000 Hz (RFC 3551).
    /// </summary>
    public int SampleRate { get; init; } = 8000;

    /// <summary>
    /// Builds parameters from negotiated SDP media parameters.
    /// Automatically sets the correct hardware sample rate for each codec.
    /// </summary>
    public static AudioConnectionParameters From(CalloraVoipSdk.Core.Domain.Calls.CallMediaParameters mp) =>
        Build(mp);

    private static AudioConnectionParameters Build(CalloraVoipSdk.Core.Domain.Calls.CallMediaParameters mp)
    {
        var codecName = NormalizeCodecName(mp.CodecName, mp.PayloadType, mp.PayloadTypeCodecMap);
        var sampleRate = IsG722(codecName) ? 16_000 : mp.ClockRate;

        return new AudioConnectionParameters
        {
            PayloadType = mp.PayloadType,
            CodecName = codecName,
            PayloadTypeCodecMap = mp.PayloadTypeCodecMap,
            ClockRate   = mp.ClockRate,
            SampleRate  = sampleRate > 0 ? sampleRate : 8_000
        };
    }

    private static string NormalizeCodecName(
        string? codecName,
        int payloadType,
        IReadOnlyDictionary<int, string>? codecMap)
    {
        if (!string.IsNullOrWhiteSpace(codecName))
            return codecName.Trim().ToUpperInvariant();

        if (codecMap is not null && codecMap.TryGetValue(payloadType, out var mapped)
            && !string.IsNullOrWhiteSpace(mapped))
        {
            return mapped.Trim().ToUpperInvariant();
        }

        return payloadType switch
        {
            0 => "PCMU",
            8 => "PCMA",
            9 => "G722",
            _ => $"PT{payloadType}"
        };
    }

    private static bool IsG722(string codecName)
        => codecName.Equals("G722", StringComparison.OrdinalIgnoreCase)
           || codecName.Equals("G.722", StringComparison.OrdinalIgnoreCase);
}
