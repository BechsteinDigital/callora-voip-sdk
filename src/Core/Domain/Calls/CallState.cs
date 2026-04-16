namespace CalloraVoipSdk.Core.Domain.Calls;

public enum CallState
{
    Idle,
    Dialing,
    Ringing,
    Connected,
    OnHold,
    Transferring,
    Terminated
}
