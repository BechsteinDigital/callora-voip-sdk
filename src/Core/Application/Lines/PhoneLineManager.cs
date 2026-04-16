using System.Collections.Concurrent;
using CalloraVoipSdk.Core.Domain.Events;
using CalloraVoipSdk.Core.Domain.Lines;

namespace CalloraVoipSdk.Core.Application.Lines;

public sealed class PhoneLineManager : IDisposable
{
    private readonly Func<SipAccount, PhoneLine> _factory;
    private readonly ConcurrentDictionary<LineId, PhoneLine> _lines = new();

    public event EventHandler<IncomingCallEventArgs>? IncomingCall;

    internal PhoneLineManager(Func<SipAccount, PhoneLine> factory)
        => _factory = factory;

    public IPhoneLine Register(SipAccount account)
    {
        var line = _factory(account);
        line.IncomingCall += (s, e) => IncomingCall?.Invoke(s, e);
        _lines[line.LineId] = line;
        line.StartRegistration();
        return line;
    }

    public async Task UnregisterAsync(LineId id, CancellationToken ct = default)
    {
        if (_lines.TryRemove(id, out var line))
        {
            await line.UnregisterAsync(ct);
            line.Dispose();
        }
    }

    public IReadOnlyCollection<IPhoneLine> All => _lines.Values.ToList<IPhoneLine>();

    public void Dispose()
    {
        foreach (var line in _lines.Values) line.Dispose();
        _lines.Clear();
    }
}
