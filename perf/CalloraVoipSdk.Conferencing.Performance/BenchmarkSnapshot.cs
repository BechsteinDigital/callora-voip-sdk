namespace CalloraVoipSdk.Conferencing.Performance;

internal sealed record BenchmarkSnapshot(
    DateTimeOffset CapturedAtUtc,
    string RuntimeVersion,
    BenchmarkResult[] Cases);
