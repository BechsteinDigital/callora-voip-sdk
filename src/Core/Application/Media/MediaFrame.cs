namespace CalloraVoipSdk.Core.Application.Media;

public readonly record struct MediaFrame(
    ReadOnlyMemory<byte> Payload,
    int PayloadType,
    uint DurationRtpUnits);
