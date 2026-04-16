using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Modules;

/// <summary>
/// SDK facade for conferencing features.
/// </summary>
public interface IConferencingModule
{
    /// <summary>True when this module can be used in the current runtime context.</summary>
    bool IsAvailable { get; }

    /// <summary>Active conference sessions.</summary>
    IReadOnlyCollection<IConferenceSession> Active { get; }

    /// <summary>Creates one conference session.</summary>
    IConferenceSession Create();
}

/// <summary>
/// SDK-level conference session contract exposed from the conferencing module.
/// </summary>
public interface IConferenceSession : IMixedMediaBus, IDisposable
{
    /// <summary>Stable conference identifier.</summary>
    string Id { get; }

    /// <summary>Current participant count.</summary>
    int ParticipantCount { get; }

    Task<ModuleOperationResult> AddParticipantAsync(ICall call, CancellationToken ct = default);

    Task<ModuleOperationResult> RemoveParticipantAsync(CallId callId, CancellationToken ct = default);

    Task<ModuleOperationResult> SetParticipantMuteAsync(CallId callId, bool isMuted, CancellationToken ct = default);

    Task<ModuleOperationResult> SetParticipantLevelAsync(CallId callId, float level, CancellationToken ct = default);

    Task<ModuleOperationResult> CloseAsync(CancellationToken ct = default);
}
