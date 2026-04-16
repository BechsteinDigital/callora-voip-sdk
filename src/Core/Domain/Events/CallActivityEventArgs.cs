using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class CallActivityEventArgs : EventArgs
{
    public ICall Call { get; }
    internal CallActivityEventArgs(ICall call) => Call = call;
}
