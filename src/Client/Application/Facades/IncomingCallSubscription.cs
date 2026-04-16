using CalloraVoipSdk.Core.Domain.Events;

namespace CalloraVoipSdk;

internal sealed class IncomingCallSubscription : IDisposable
{
    private readonly VoipClient _client;
    private EventHandler<IncomingCallEventArgs>? _handler;

    public IncomingCallSubscription(VoipClient client, EventHandler<IncomingCallEventArgs> handler)
    {
        _client = client;
        _handler = handler;
    }

    public void Dispose()
    {
        var handler = Interlocked.Exchange(ref _handler, null);
        if (handler is not null)
            _client.IncomingCall -= handler;
    }
}
