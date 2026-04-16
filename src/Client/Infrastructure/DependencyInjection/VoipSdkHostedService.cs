using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
namespace CalloraVoipSdk.DependencyInjection;

/// <summary>
/// Coordinates CalloraVoipSdk lifecycle through host start/stop hooks.
/// </summary>
public sealed class CalloraHostedService : IHostedService
{
    private readonly IVoipClient _client;
    private readonly ILogger<CalloraHostedService> _logger;

    /// <summary>
    /// Creates the hosted service wrapper.
    /// </summary>
    public CalloraHostedService(IVoipClient client, ILogger<CalloraHostedService>? logger = null)
    {
        _client = client;
        _logger = logger ?? NullLogger<CalloraHostedService>.Instance;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_client is VoipClient concreteClient)
        {
            return concreteClient.StartRuntimeAsync(cancellationToken);
        }

        _logger.LogInformation("CalloraVoipSdk hosted lifecycle started.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is VoipClient concreteClient)
        {
            try
            {
                await concreteClient.StopRuntimeAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Runtime shutdown failed.");
            }
        }

        _client.Dispose();
        _logger.LogInformation("CalloraVoipSdk hosted lifecycle stopped.");
    }
}
