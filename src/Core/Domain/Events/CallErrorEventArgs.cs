using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class CallErrorEventArgs : EventArgs
{
    public string     Message   { get; }
    public ICall?     Call      { get; }
    public Exception? Exception { get; }

    internal CallErrorEventArgs(string message, ICall? call = null, Exception? ex = null)
        => (Message, Call, Exception) = (message, call, ex);
}
