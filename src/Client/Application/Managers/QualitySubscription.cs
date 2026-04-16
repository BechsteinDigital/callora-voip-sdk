namespace CalloraVoipSdk;

internal sealed class QualitySubscription(Action dispose) : IDisposable
{
    private Action? _dispose = dispose;

    public void Dispose()
    {
        Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}
