namespace CalloraVoipSdk.Conferencing.Performance;

internal sealed record BenchmarkResult(
    string Name,
    double MeanNanosecondsPerOp,
    double MeanAllocatedBytesPerOp,
    int OperationsPerIteration,
    int Iterations);
