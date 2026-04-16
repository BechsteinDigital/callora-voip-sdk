namespace CalloraVoipSdk.Core.Application.Media;

public sealed class MediaFrameReceivedEventArgs : EventArgs
{
    public MediaFrame Frame { get; }

    public MediaFrameReceivedEventArgs(MediaFrame frame) => Frame = frame;
}
