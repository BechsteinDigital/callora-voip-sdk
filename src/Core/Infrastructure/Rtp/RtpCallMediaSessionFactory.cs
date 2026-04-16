using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Infrastructure.Rtp;

/// <summary>
/// Infrastructure implementation of <see cref="ICallMediaSessionFactory"/>.
/// Creates <see cref="RtpCallMediaSession"/> instances from negotiated SDP parameters.
/// Registered in the SDK facade (<see cref="Sdk.VoipClient"/>) to satisfy the Application port.
/// </summary>
internal sealed class RtpCallMediaSessionFactory : ICallMediaSessionFactory
{
    private readonly ILoggerFactory _loggerFactory;

    internal RtpCallMediaSessionFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public ICallMediaSession Create(CallMediaParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        return new RtpCallMediaSession(parameters, _loggerFactory);
    }
}
