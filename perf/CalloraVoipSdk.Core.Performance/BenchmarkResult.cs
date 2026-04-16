namespace CalloraVoipSdk.Performance;

/// <summary>
/// Serializable metrics for one benchmark case.
/// </summary>
internal sealed record BenchmarkResult(
    string Name,
    double MeanNanosecondsPerOp,
    double MeanAllocatedBytesPerOp,
    int OperationsPerIteration,
    int Iterations);
