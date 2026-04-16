namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Error payload raised by recording and playback sessions.
/// </summary>
public sealed class MediaSessionErrorEventArgs : EventArgs
{
    /// <summary>
    /// Creates one media session error event argument instance.
    /// </summary>
    public MediaSessionErrorEventArgs(string operation, string message, Exception exception)
    {
        Operation = operation;
        Message = message;
        Exception = exception;
    }

    /// <summary>
    /// Logical operation that failed (for example record-write, playback-send).
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Short human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Original exception that caused the failure.
    /// </summary>
    public Exception Exception { get; }
}
