using CalloraVoipSdk.Core.Domain.Lines;

namespace CalloraVoipSdk.Core.Domain.Events;

/// <summary>
/// Raised when a phone line begins a reconnect attempt after its SIP registration was lost.
/// The line is already in <see cref="LineState.Reconnecting"/> when this event fires.
/// </summary>
public sealed class LineReconnectingEventArgs : EventArgs
{
    /// <summary>
    /// The one-based sequential number of this reconnect attempt.
    /// Resets to 1 on each call to <c>StartRegistration</c>.
    /// </summary>
    public int AttemptNumber { get; }

    /// <summary>The phone line that is attempting to reconnect.</summary>
    public IPhoneLine Line { get; }

    internal LineReconnectingEventArgs(int attemptNumber, IPhoneLine line)
        => (AttemptNumber, Line) = (attemptNumber, line);
}
