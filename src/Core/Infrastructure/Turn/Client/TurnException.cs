namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Exception raised when a TURN transaction fails.
/// </summary>
internal class TurnException : Exception
{
    /// <summary>Initialises with a detail message.</summary>
    public TurnException(string message) : base(message)
    {
    }

    /// <summary>Initialises with a detail message and inner exception.</summary>
    public TurnException(string message, Exception inner) : base(message, inner)
    {
    }
}
