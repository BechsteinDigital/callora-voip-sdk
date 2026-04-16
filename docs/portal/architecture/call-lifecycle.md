# Call Lifecycle

## Outbound Call State Machine

```
[Dialing] ──INVITE sent──► [Ringing] ──200 OK──► [Connected]
                                                       │
                                                [OnHold] ◄──► [Connected]
                                                       │
                                                 [Terminated]
```

## Inbound Call State Machine

```
[Ringing] ──AcceptAsync()──► [Connected] ──► [Terminated]
[Ringing] ──RejectAsync()──► [Terminated]
[Ringing] ──RedirectAsync()──► [Terminated]
```

## Important Rules

- Call state changes are always delivered **on a background thread**.
  For happy-path flows you can use `DialAndWaitUntilConnectedAsync` / `ConnectAsync`.
  For advanced orchestration you can still bridge with `TaskCompletionSource`.
- Calling an action (e.g. `HoldAsync`) in the wrong state throws `InvalidOperationException`.
- After `Terminated`, all references to the `ICall` object become inert. Do not reuse.
