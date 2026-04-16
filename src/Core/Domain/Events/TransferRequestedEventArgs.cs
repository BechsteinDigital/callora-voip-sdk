using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class TransferRequestedEventArgs : EventArgs
{
    public string TargetUri { get; }
    public ICall  Call      { get; }
    /// <summary>Set to <c>true</c> in the event handler to accept the transfer.</summary>
    public bool   Accept    { get; set; }

    internal TransferRequestedEventArgs(string targetUri, ICall call)
        => (TargetUri, Call) = (targetUri, call);
}
