namespace CalloraVoipSdk.Performance;

/// <summary>
/// Serializable benchmark snapshot persisted to baseline JSON.
/// </summary>
internal sealed record BenchmarkSnapshot(
    DateTimeOffset CapturedAtUtc,
    string RuntimeVersion,
    string FrameworkDescription,
    IReadOnlyList<BenchmarkResult> Cases);
