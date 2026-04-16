using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class IncomingCallEventArgs : EventArgs
{
    public ICall Call { get; }
    internal IncomingCallEventArgs(ICall call) => Call = call;
}
