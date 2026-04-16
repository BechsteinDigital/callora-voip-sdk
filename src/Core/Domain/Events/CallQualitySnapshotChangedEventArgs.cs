using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Domain.Events;

/// <summary>
/// Event arguments for call quality snapshot updates.
/// </summary>
public sealed class CallQualitySnapshotChangedEventArgs : EventArgs
{
    /// <summary>
    /// Creates event arguments for one quality update.
    /// </summary>
    internal CallQualitySnapshotChangedEventArgs(CallQualitySnapshot snapshot, ICall call)
    {
        Snapshot = snapshot;
        Call = call;
    }

    /// <summary>
    /// Latest call quality snapshot.
    /// </summary>
    public CallQualitySnapshot Snapshot { get; }

    /// <summary>
    /// Call that emitted this quality update.
    /// </summary>
    public ICall Call { get; }
}
