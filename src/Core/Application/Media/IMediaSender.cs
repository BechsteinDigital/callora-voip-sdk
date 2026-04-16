using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

public interface IMediaSender : IDisposable
{
    void AttachToCall(ICall call);
    void Detach();
    Task SendAsync(MediaFrame frame, CancellationToken ct = default);
}
