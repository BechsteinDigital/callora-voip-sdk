# Making Calls

## Outbound Call (Convenience)

```csharp
using CalloraVoipSdk.Core.Domain.Calls;

var dialResult = await client.DialAndWaitUntilConnectedAsync(
    line,
    "sip:1002@pbx.example.com",
    new DialWaitOptions
    {
        ConnectTimeout = TimeSpan.FromSeconds(30),
        HangupOnTimeout = true,
        HangupOnCancellation = true
    });

if (!dialResult.IsSuccess || dialResult.Call is null)
    throw new InvalidOperationException($"Dial failed: {dialResult.Status}");

var call = dialResult.Call;
```

## Outbound Call (Advanced Event-Driven Flow)

```csharp
var call = await line.DialAsync("sip:1002@pbx.example.com");
call.StateChanged += (_, e) =>
    Console.WriteLine($"Call: {e.OldState} → {e.NewState}");
```

## Inbound Call

```csharp
using var incomingSubscription = client.OnIncomingCall(async call =>
{
    Console.WriteLine($"Inbound from: {call.RemoteParty}");

    // Reject busy
    if (IsAgentBusy())
    {
        await call.RejectAsync(486, "Busy Here");
        return;
    }

    // Redirect to queue
    if (ShouldForwardToQueue(call))
    {
        await call.RedirectAsync(["sip:queue@pbx.example.com"], statusCode: 302);
        return;
    }

    await call.AcceptAsync();
});
```

## Call Control

```csharp
// DTMF
await call.SendDtmfAsync(new DtmfTone('5'));

// Hold / Unhold
await call.HoldAsync();
await call.UnholdAsync();

// Blind Transfer
await call.BlindTransferAsync("sip:supervisor@pbx.example.com");

// Attended Transfer (both calls must be connected)
await call.AttendedTransferAsync(consultCall);

// Hangup
await call.HangupAsync();
```

## Call States

| State | Meaning |
|-------|---------|
| `Dialing` | INVITE sent, awaiting provisional response |
| `Ringing` | 180 Ringing received |
| `Connected` | 200 OK + ACK complete — media active |
| `OnHold` | re-INVITE hold completed |
| `Transferring` | Transfer in progress |
| `Terminated` | Dialog closed |

> Only invoke call actions in the appropriate state.
> Calling `HoldAsync()` on a `Terminated` call throws `InvalidOperationException`.

## Next Step

→ [Media & Audio](media-and-audio.md)
