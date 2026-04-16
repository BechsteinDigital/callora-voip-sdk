namespace CalloraVoipSdk.Core.Domain.Calls;

internal static class CallStateRules
{
    public static bool CanTransition(CallState from, CallState to)
    {
        if (from == to) return true;
        if (from == CallState.Terminated) return false;

        return from switch
        {
            CallState.Idle         => to is CallState.Dialing or CallState.Ringing or CallState.Terminated,
            CallState.Dialing      => to is CallState.Ringing or CallState.Connected or CallState.Terminated,
            CallState.Ringing      => to is CallState.Connected or CallState.Terminated,
            CallState.Connected    => to is CallState.OnHold or CallState.Transferring or CallState.Terminated,
            CallState.OnHold       => to is CallState.Connected or CallState.Transferring or CallState.Terminated,
            CallState.Transferring => to is CallState.Connected or CallState.Terminated,
            _                      => false
        };
    }
}
