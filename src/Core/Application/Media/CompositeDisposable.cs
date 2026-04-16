namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Disposes two resources together in deterministic order.
/// </summary>
internal sealed class CompositeDisposable : IDisposable
{
    private readonly IDisposable _first;
    private readonly IDisposable _second;
    private int _disposed;

    /// <summary>
    /// Creates a paired disposable wrapper.
    /// </summary>
    internal CompositeDisposable(IDisposable first, IDisposable second)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
    }

    /// <summary>
    /// Disposes both wrapped resources once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _first.Dispose();
        _second.Dispose();
    }
}
