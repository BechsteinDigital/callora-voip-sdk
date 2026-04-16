using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// One decoded frame read from a file codec plus playback timing hint.
/// </summary>
internal readonly record struct AudioFileFrame(MediaFrame Frame, TimeSpan Delay);
