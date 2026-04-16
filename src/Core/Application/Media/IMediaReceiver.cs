using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

public interface IMediaReceiver : IDisposable
{
    event EventHandler<MediaFrameReceivedEventArgs>? FrameReceived;

    void AttachToCall(ICall call);
    void Detach();
}
