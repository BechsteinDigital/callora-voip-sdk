namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Policy applied when the server reaches its maximum number of concurrent stream connections.
/// </summary>
internal enum StunConnectionCapPolicy
{
    /// <summary>
    /// Applies backpressure by waiting for a free slot before accepting another connection.
    /// New inbound connections queue in the OS listener backlog.
    /// </summary>
    Backpressure,

    /// <summary>
    /// Accepts and immediately closes new inbound connections while the cap is reached.
    /// </summary>
    RejectNew
}
