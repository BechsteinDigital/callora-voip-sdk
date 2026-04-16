namespace CalloraVoipSdk;

/// <summary>
/// Signals that the VoIP SDK client could not be initialized due to runtime environment constraints.
/// </summary>
public sealed class VoipClientInitializationException : InvalidOperationException
{
    public VoipClientInitializationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
