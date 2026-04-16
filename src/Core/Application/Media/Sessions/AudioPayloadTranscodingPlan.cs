using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Prepared transcoding delegates between call/conference payload and file payload.
/// </summary>
internal sealed class AudioPayloadTranscodingPlan
{
    /// <summary>
    /// Creates one transcoding plan instance.
    /// </summary>
    public AudioPayloadTranscodingPlan(
        AudioFileCodecContext codecContext,
        Func<MediaFrame, MediaFrame> toFileFrame,
        Func<MediaFrame, MediaFrame> fromFileFrame)
    {
        CodecContext = codecContext;
        ToFileFrame = toFileFrame;
        FromFileFrame = fromFileFrame;
    }

    /// <summary>
    /// File codec context resolved for this plan.
    /// </summary>
    public AudioFileCodecContext CodecContext { get; }

    /// <summary>
    /// Converts one inbound media frame into file payload representation.
    /// </summary>
    public Func<MediaFrame, MediaFrame> ToFileFrame { get; }

    /// <summary>
    /// Converts one file payload frame into outbound media representation.
    /// </summary>
    public Func<MediaFrame, MediaFrame> FromFileFrame { get; }
}
