namespace CalloraVoipSdk.Core.Domain.Calls;

internal readonly record struct CallAudioFrame(
    byte[] Payload,
    int PayloadType,
    uint DurationRtpUnits);
