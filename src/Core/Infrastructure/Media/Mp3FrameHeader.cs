namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal readonly record struct Mp3FrameHeader(int FrameLengthBytes, int SampleRateHz, int SamplesPerFrame);
