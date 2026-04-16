using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Modules;

/// <summary>
/// SDK facade for playback features.
/// </summary>
public interface IPlaybackModule
{
    /// <summary>True when this module can be used in the current runtime context.</summary>
    bool IsAvailable { get; }

    /// <summary>Active playback sessions.</summary>
    IReadOnlyCollection<IPlaybackSession> Active { get; }

    Task<IPlaybackSession> StartCallAsync(ICall call, PlaybackRequest request, CancellationToken ct = default);

    Task<IPlaybackSession> StartMixedBusAsync(IMixedMediaBus bus, PlaybackRequest request, CancellationToken ct = default);
}
