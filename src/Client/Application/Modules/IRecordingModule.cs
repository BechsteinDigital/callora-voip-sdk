using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Modules;

/// <summary>
/// SDK facade for recording features.
/// </summary>
public interface IRecordingModule
{
    /// <summary>True when this module can be used in the current runtime context.</summary>
    bool IsAvailable { get; }

    /// <summary>Active recording sessions.</summary>
    IReadOnlyCollection<IRecordingSession> Active { get; }

    Task<IRecordingSession> StartCallAsync(ICall call, RecordingOptions? options = null, CancellationToken ct = default);

    Task<IRecordingSession> StartMixedBusAsync(IMixedMediaBus bus, RecordingOptions? options = null, CancellationToken ct = default);
}
