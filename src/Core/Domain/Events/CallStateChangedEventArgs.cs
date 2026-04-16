using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class CallStateChangedEventArgs : EventArgs
{
    public CallState OldState { get; }
    public CallState NewState { get; }
    public ICall     Call     { get; }

    internal CallStateChangedEventArgs(CallState old, CallState next, ICall call)
        => (OldState, NewState, Call) = (old, next, call);
}
