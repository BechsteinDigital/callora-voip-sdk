namespace CalloraVoipSdk.Core.Infrastructure.Common.Disposal;

/// <summary>
/// Async-disposable adapter for deterministic cleanup of synchronous actions.
/// </summary>
internal sealed class AsyncDisposeAction : IAsyncDisposable
{
    private readonly Action _dispose;
    private int _disposed;

    /// <summary>
    /// Creates an async-disposable wrapper for one synchronous cleanup action.
    /// </summary>
    public AsyncDisposeAction(Action dispose) => _dispose = dispose;

    /// <summary>
    /// Runs the cleanup action exactly once.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return ValueTask.CompletedTask;
        _dispose();
        return ValueTask.CompletedTask;
    }
}
