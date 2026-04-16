namespace CalloraVoipSdk.Core.Infrastructure.Srtp.Context;

/// <summary>
/// Thrown when an SRTP packet's index falls within the replay window and has already been
/// received, indicating a replay attack (RFC 3711 §3.3).
/// </summary>
internal sealed class SrtpReplayException : Exception
{
    public SrtpReplayException(string message) : base(message) { }
}
