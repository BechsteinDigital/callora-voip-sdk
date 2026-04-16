using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class DtmfReceivedEventArgs : EventArgs
{
    public DtmfTone Tone       { get; }
    public int      DurationMs { get; }
    public ICall    Call       { get; }

    internal DtmfReceivedEventArgs(DtmfTone tone, int durationMs, ICall call)
        => (Tone, DurationMs, Call) = (tone, durationMs, call);
}
