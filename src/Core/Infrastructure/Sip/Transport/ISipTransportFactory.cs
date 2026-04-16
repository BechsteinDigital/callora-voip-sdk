using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Security;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

/// <summary>
/// Factory contract for creating SIP signaling transports.
/// Enables dependency injection to swap transport implementations.
/// </summary>
internal interface ISipTransportFactory
{
    /// <summary>
    /// Creates one SIP transport runtime configured for the current SDK instance.
    /// </summary>
    ISipTransportRuntime Create(TlsConfiguration? tls, ILoggerFactory loggerFactory);
}
