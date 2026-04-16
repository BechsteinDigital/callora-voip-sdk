using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class HoldStateChangedEventArgs : EventArgs
{
    public bool  IsOnHold      { get; }
    public bool  ByRemoteParty { get; }
    public ICall Call          { get; }

    internal HoldStateChangedEventArgs(bool isOnHold, bool byRemote, ICall call)
        => (IsOnHold, ByRemoteParty, Call) = (isOnHold, byRemote, call);
}
