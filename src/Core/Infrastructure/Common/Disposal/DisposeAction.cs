namespace CalloraVoipSdk.Core.Infrastructure.Common.Disposal;

/// <summary>
/// Disposable adapter that executes one synchronous cleanup action exactly once.
/// </summary>
internal sealed class DisposeAction : IDisposable
{
    private readonly Action _dispose;
    private int _disposed;

    /// <summary>
    /// Creates a disposable wrapper for one synchronous cleanup action.
    /// </summary>
    public DisposeAction(Action dispose) => _dispose = dispose;

    /// <summary>
    /// Executes the cleanup action once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _dispose();
    }
}
