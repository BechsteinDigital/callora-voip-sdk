namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// Format/context metadata provided when opening audio file codecs.
/// </summary>
internal readonly record struct AudioFileCodecContext(
    int PayloadType,
    int ClockRate,
    int SampleRate,
    int SamplesPerFrame,
    string CodecName);
