using System.Text.Json;
using CalloraVoipSdk.Conferencing.Application;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Conferencing.Performance;

internal static class Program
{
    private const double DefaultMaxRegressionPercent = 15.0;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static int Main(string[] args)
    {
        try
        {
            var options = CliOptions.Parse(args);
            var snapshot = RunBenchmarks();
            PrintResults(snapshot);

            if (!string.IsNullOrWhiteSpace(options.WriteBaselinePath))
                WriteBaseline(snapshot, options.WriteBaselinePath);

            if (string.IsNullOrWhiteSpace(options.GateBaselinePath))
                return 0;

            return EvaluateGate(snapshot, options.GateBaselinePath, options.MaxRegressionPercent);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Conferencing perf runner failed: {ex.Message}");
            return 1;
        }
    }

    private static BenchmarkSnapshot RunBenchmarks()
    {
        return new BenchmarkSnapshot(
            CapturedAtUtc: DateTimeOffset.UtcNow,
            RuntimeVersion: Environment.Version.ToString(),
            Cases:
            [
                RunMixForTargetCase(participants: 2),
                RunMixForTargetCase(participants: 4),
                RunMixForTargetCase(participants: 8),
            ]);
    }

    private static BenchmarkResult RunMixForTargetCase(int participants)
    {
        var payloadLength = 320;
        var callIds = Enumerable.Range(0, participants).Select(_ => CallId.New()).ToArray();
        var contributions = new ConferenceMixContribution[participants];
        for (var i = 0; i < participants; i++)
        {
            var payload = CreatePayload(payloadLength, (short)(1000 + (i * 100)));
            contributions[i] = new ConferenceMixContribution(callIds[i], payload, payloadLength, 1.0f);
        }

        return RunCase(
            $"conference.mix.n{participants}",
            operationsPerIteration: 50_000,
            iterations: 6,
            action: () =>
            {
                if (!Pcm16ConferenceMixer.TryMixForTarget(contributions, callIds[0], payloadLength, out var output, out var outputLength))
                    throw new InvalidOperationException("Mixer returned no output.");
                if (outputLength != payloadLength)
                    throw new InvalidOperationException("Unexpected mixed payload length.");
                Pcm16ConferenceMixer.ReturnBuffer(output);
            });
    }

    private static BenchmarkResult RunCase(string name, int operationsPerIteration, int iterations, Action action)
    {
        const int warmups = 2;
        for (var w = 0; w < warmups; w++)
        {
            for (var i = 0; i < operationsPerIteration; i++)
                action();
        }

        double totalNs = 0;
        double totalBytes = 0;
        for (var it = 0; it < iterations; it++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var before = GC.GetAllocatedBytesForCurrentThread();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            for (var op = 0; op < operationsPerIteration; op++)
                action();

            sw.Stop();
            var after = GC.GetAllocatedBytesForCurrentThread();

            totalNs += sw.Elapsed.TotalMilliseconds * 1_000_000d / operationsPerIteration;
            totalBytes += Math.Max(0, after - before) / (double)operationsPerIteration;
        }

        return new BenchmarkResult(name, totalNs / iterations, totalBytes / iterations, operationsPerIteration, iterations);
    }

    private static byte[] CreatePayload(int bytes, short seed)
    {
        var payload = new byte[bytes];
        var sampleCount = bytes / 2;
        for (var i = 0; i < sampleCount; i++)
        {
            var sample = (short)(seed + (i % 23));
            payload[i * 2] = (byte)(sample & 0xFF);
            payload[(i * 2) + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return payload;
    }

    private static int EvaluateGate(BenchmarkSnapshot current, string baselinePath, double maxRegressionPercent)
    {
        if (!File.Exists(baselinePath))
            throw new FileNotFoundException("Baseline file not found.", baselinePath);

        var baseline = JsonSerializer.Deserialize<BenchmarkSnapshot>(File.ReadAllText(baselinePath), JsonOptions)
            ?? throw new InvalidOperationException("Baseline could not be parsed.");

        var currentByName = current.Cases.ToDictionary(c => c.Name, StringComparer.Ordinal);
        var failures = new List<string>();
        foreach (var baselineCase in baseline.Cases)
        {
            if (!currentByName.TryGetValue(baselineCase.Name, out var currentCase))
            {
                failures.Add($"Missing benchmark case '{baselineCase.Name}'.");
                continue;
            }

            ValidateMetric(failures, baselineCase.Name, "time.ns/op", currentCase.MeanNanosecondsPerOp, baselineCase.MeanNanosecondsPerOp, maxRegressionPercent);
            ValidateMetric(failures, baselineCase.Name, "alloc.bytes/op", currentCase.MeanAllocatedBytesPerOp, baselineCase.MeanAllocatedBytesPerOp, maxRegressionPercent);
        }

        if (failures.Count == 0)
        {
            Console.WriteLine($"Regression gate passed (max regression {maxRegressionPercent:F2}%).");
            return 0;
        }

        Console.Error.WriteLine($"Regression gate failed (max regression {maxRegressionPercent:F2}%):");
        foreach (var failure in failures)
            Console.Error.WriteLine($"- {failure}");
        return 1;
    }

    private static void ValidateMetric(
        ICollection<string> failures,
        string caseName,
        string metricName,
        double currentValue,
        double baselineValue,
        double maxRegressionPercent)
    {
        if (baselineValue <= 0)
        {
            if (baselineValue == 0 && currentValue == 0)
                return;
            failures.Add($"Case '{caseName}' has invalid baseline metric '{metricName}'.");
            return;
        }

        var allowed = baselineValue * (1 + maxRegressionPercent / 100.0);
        if (currentValue > allowed)
        {
            failures.Add(
                $"Case '{caseName}' metric '{metricName}' is {currentValue:F2} (baseline {baselineValue:F2}, allowed <= {allowed:F2}).");
        }
    }

    private static void PrintResults(BenchmarkSnapshot snapshot)
    {
        Console.WriteLine($"Conferencing perf snapshot @ {snapshot.CapturedAtUtc:O} (.NET {snapshot.RuntimeVersion})");
        foreach (var result in snapshot.Cases)
        {
            Console.WriteLine(
                $"{result.Name,-24} time={result.MeanNanosecondsPerOp,10:F2} ns/op  alloc={result.MeanAllocatedBytesPerOp,8:F2} B/op");
        }
    }

    private static void WriteBaseline(BenchmarkSnapshot snapshot, string baselinePath)
    {
        var dir = Path.GetDirectoryName(baselinePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(baselinePath, JsonSerializer.Serialize(snapshot, JsonOptions));
        Console.WriteLine($"Baseline written to {baselinePath}");
    }

}
