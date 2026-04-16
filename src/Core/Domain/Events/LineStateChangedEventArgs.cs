using CalloraVoipSdk.Core.Domain.Lines;

namespace CalloraVoipSdk.Core.Domain.Events;

public sealed class LineStateChangedEventArgs : EventArgs
{
    public LineState  OldState { get; }
    public LineState  NewState { get; }
    public IPhoneLine Line     { get; }

    internal LineStateChangedEventArgs(LineState old, LineState next, IPhoneLine line)
        => (OldState, NewState, Line) = (old, next, line);
}
