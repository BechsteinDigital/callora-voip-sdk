using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using CalloraVoipSdk.Core.Infrastructure.Rtp.Packets;
using CalloraVoipSdk.Core.Infrastructure.Rtp.Wire;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;
using CalloraVoipSdk.Core.Infrastructure.Srtp.Context;
using CalloraVoipSdk.Core.Infrastructure.Srtp.Crypto;

namespace CalloraVoipSdk.Performance;

/// <summary>
/// Runs allocation/time microbenchmarks for CORE-010 hot paths and optionally
/// compares results against a persisted baseline.
/// </summary>
internal static class Program
{
    private const double DefaultMaxRegressionPercent = 15.0;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Entry point.
    /// </summary>
    public static int Main(string[] args)
    {
        try
        {
            var options = CliOptions.Parse(args);
            var snapshot = RunBenchmarks();
            PrintResults(snapshot);

            if (options.WriteBaselinePath is not null)
                WriteBaseline(snapshot, options.WriteBaselinePath);

            if (options.GateBaselinePath is null)
                return 0;

            return EvaluateGate(snapshot, options.GateBaselinePath, options.MaxRegressionPercent);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Performance runner failed: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Executes all benchmark cases and returns one snapshot.
    /// </summary>
    private static BenchmarkSnapshot RunBenchmarks()
    {
        var cases = new[]
        {
            RunSrtpProtectUnprotectRoundTrip(),
            RunSipWireFramerRoundTrip(),
            RunRtpPacketCodecDecode()
        };

        return new BenchmarkSnapshot(
            CapturedAtUtc: DateTimeOffset.UtcNow,
            RuntimeVersion: Environment.Version.ToString(),
            FrameworkDescription: RuntimeInformation.FrameworkDescription,
            Cases: cases);
    }

    /// <summary>
    /// Measures SRTP protect/unprotect round-trip costs.
    /// </summary>
    private static BenchmarkResult RunSrtpProtectUnprotectRoundTrip()
    {
        var protectContext = BuildSrtpContext();
        var unprotectContext = BuildSrtpContext();
        ushort sequence = 1;

        return RunCase(
            "srtp.protect_unprotect.roundtrip",
            operationsPerIteration: 3_000,
            iterations: 6,
            action: () =>
            {
                var packet = BuildRtpPacket(sequence);
                sequence = unchecked((ushort)(sequence + 1));
                if (sequence == 0)
                    sequence = 1;

                var protectedPacket = protectContext.Protect(packet);
                var restoredPacket = unprotectContext.Unprotect(protectedPacket);
                if (restoredPacket.Length != packet.Length)
                    throw new InvalidOperationException("Unexpected SRTP round-trip payload size.");
            });
    }

    /// <summary>
    /// Measures SIP stream framing for one complete buffered message.
    /// </summary>
    private static BenchmarkResult RunSipWireFramerRoundTrip()
    {
        var framer = new SipWireStreamFramer();
        var message = BuildSipStreamMessage(contentLength: 120);

        return RunCase(
            "sip.stream_framer.frame_parse",
            operationsPerIteration: 8_000,
            iterations: 6,
            action: () =>
            {
                framer.Append(message);
                if (!framer.TryReadFrame(out var frame))
                    throw new InvalidOperationException("SIP frame was not produced although complete bytes were appended.");
                if (frame.Length != message.Length)
                    throw new InvalidOperationException("Unexpected SIP frame length.");
            });
    }

    /// <summary>
    /// Measures RTP decode costs on representative payload size.
    /// </summary>
    private static BenchmarkResult RunRtpPacketCodecDecode()
    {
        var codec = new RtpPacketCodec();
        var payload = new byte[1200];
        for (var i = 0; i < payload.Length; i++)
            payload[i] = (byte)(i & 0xFF);

        var datagram = codec.Encode(new RtpPacket
        {
            PayloadType = 0,
            SequenceNumber = 100,
            Timestamp = 160,
            Ssrc = 0x1234_5678,
            Payload = payload
        });

        return RunCase(
            "rtp.packet_codec.decode",
            operationsPerIteration: 100_000,
            iterations: 6,
            action: () =>
            {
                var packet = codec.Decode(datagram);
                if (packet.Payload.Length != payload.Length)
                    throw new InvalidOperationException("Unexpected RTP payload size after decode.");
            });
    }

    /// <summary>
    /// Runs one benchmark case and returns mean time/allocation metrics.
    /// </summary>
    private static BenchmarkResult RunCase(
        string name,
        int operationsPerIteration,
        int iterations,
        Action action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(operationsPerIteration, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(iterations, 0);
        ArgumentNullException.ThrowIfNull(action);

        const int warmupIterations = 2;
        for (var warmup = 0; warmup < warmupIterations; warmup++)
        {
            for (var i = 0; i < operationsPerIteration; i++)
                action();
        }

        double timeSumNsPerOp = 0;
        double allocSumBytesPerOp = 0;

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();

            for (var operation = 0; operation < operationsPerIteration; operation++)
                action();

            stopwatch.Stop();
            var allocatedAfter = GC.GetAllocatedBytesForCurrentThread();
            var allocatedBytes = Math.Max(0, allocatedAfter - allocatedBefore);

            timeSumNsPerOp += stopwatch.Elapsed.TotalMilliseconds * 1_000_000d / operationsPerIteration;
            allocSumBytesPerOp += (double)allocatedBytes / operationsPerIteration;
        }

        return new BenchmarkResult(
            Name: name,
            MeanNanosecondsPerOp: timeSumNsPerOp / iterations,
            MeanAllocatedBytesPerOp: allocSumBytesPerOp / iterations,
            OperationsPerIteration: operationsPerIteration,
            Iterations: iterations);
    }

    /// <summary>
    /// Evaluates current benchmark results against persisted baseline thresholds.
    /// </summary>
    private static int EvaluateGate(BenchmarkSnapshot current, string baselinePath, double maxRegressionPercent)
    {
        if (!File.Exists(baselinePath))
            throw new FileNotFoundException($"Baseline file not found: {baselinePath}", baselinePath);

        var baseline = JsonSerializer.Deserialize<BenchmarkSnapshot>(File.ReadAllText(baselinePath), JsonOptions)
            ?? throw new InvalidOperationException("Baseline file could not be parsed.");

        var currentByName = current.Cases.ToDictionary(c => c.Name, StringComparer.Ordinal);
        var failures = new List<string>();

        foreach (var baselineCase in baseline.Cases)
        {
            if (!currentByName.TryGetValue(baselineCase.Name, out var currentCase))
            {
                failures.Add($"Case '{baselineCase.Name}' is missing in current benchmark run.");
                continue;
            }

            ValidateMetric(
                failures,
                baselineCase.Name,
                metricName: "time.ns/op",
                currentValue: currentCase.MeanNanosecondsPerOp,
                baselineValue: baselineCase.MeanNanosecondsPerOp,
                maxRegressionPercent);

            ValidateMetric(
                failures,
                baselineCase.Name,
                metricName: "alloc.bytes/op",
                currentValue: currentCase.MeanAllocatedBytesPerOp,
                baselineValue: baselineCase.MeanAllocatedBytesPerOp,
                maxRegressionPercent);
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

    /// <summary>
    /// Validates one metric against baseline plus allowed regression percentage.
    /// </summary>
    private static void ValidateMetric(
        ICollection<string> failures,
        string caseName,
        string metricName,
        double currentValue,
        double baselineValue,
        double maxRegressionPercent)
    {
        if (baselineValue < 0)
        {
            failures.Add($"Case '{caseName}' has invalid baseline metric '{metricName}' value {baselineValue:F3}.");
            return;
        }

        if (baselineValue == 0)
        {
            if (currentValue > 0)
            {
                failures.Add(
                    $"Case '{caseName}' metric '{metricName}' regressed from baseline 0 to {currentValue:F3}.");
            }

            return;
        }

        var allowed = baselineValue * (1 + maxRegressionPercent / 100.0);
        if (currentValue <= allowed)
            return;

        failures.Add(
            $"Case '{caseName}' metric '{metricName}' is {currentValue:F3} (baseline {baselineValue:F3}, allowed <= {allowed:F3}).");
    }

    /// <summary>
    /// Persists a benchmark snapshot as baseline JSON.
    /// </summary>
    private static void WriteBaseline(BenchmarkSnapshot snapshot, string baselinePath)
    {
        var directory = Path.GetDirectoryName(baselinePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(snapshot, JsonOptions);
        File.WriteAllText(baselinePath, json);
        Console.WriteLine($"Baseline written to {baselinePath}");
    }

    /// <summary>
    /// Prints benchmark results in a compact table format.
    /// </summary>
    private static void PrintResults(BenchmarkSnapshot snapshot)
    {
        Console.WriteLine(
            $"Performance snapshot @ {snapshot.CapturedAtUtc:O} ({snapshot.FrameworkDescription}, .NET {snapshot.RuntimeVersion})");

        foreach (var result in snapshot.Cases)
        {
            Console.WriteLine(
                $"{result.Name,-36} time={result.MeanNanosecondsPerOp,10:F2} ns/op  alloc={result.MeanAllocatedBytesPerOp,8:F2} B/op");
        }
    }

    /// <summary>
    /// Builds one complete SIP stream message with explicit Content-Length body.
    /// </summary>
    private static byte[] BuildSipStreamMessage(int contentLength)
    {
        if (contentLength < 0)
            throw new ArgumentOutOfRangeException(nameof(contentLength), contentLength, "Content length must be >= 0.");

        var body = new string('x', contentLength);
        var message =
            "MESSAGE sip:bob@example.org SIP/2.0\r\n" +
            "Via: SIP/2.0/TCP host1;branch=z9hG4bK-perf\r\n" +
            "From: <sip:alice@example.org>;tag=abc\r\n" +
            "To: <sip:bob@example.org>\r\n" +
            "Call-ID: call-perf\r\n" +
            "CSeq: 9 MESSAGE\r\n" +
            "Content-Type: text/plain\r\n" +
            $"Content-Length: {contentLength}\r\n" +
            "\r\n" +
            body;

        return Encoding.UTF8.GetBytes(message);
    }

    /// <summary>
    /// Creates a deterministic SRTP context used by the perf case.
    /// </summary>
    private static SrtpContext BuildSrtpContext()
    {
        var keyMaterialRaw = new byte[30];
        for (var i = 0; i < keyMaterialRaw.Length; i++)
            keyMaterialRaw[i] = (byte)(i + 1);

        var inline = "inline:" + Convert.ToBase64String(keyMaterialRaw);
        var material = SrtpKeyMaterial.ParseInline(inline, SrtpCryptoSuite.AesCm128HmacSha1_80);
        return new SrtpContext(material);
    }

    /// <summary>
    /// Builds a deterministic RTP packet payload for perf runs.
    /// </summary>
    private static byte[] BuildRtpPacket(ushort sequenceNumber)
    {
        var payloadLength = 160;
        var packet = new byte[12 + payloadLength];
        packet[0] = 0x80;
        packet[1] = 0x00;
        packet[2] = (byte)(sequenceNumber >> 8);
        packet[3] = (byte)sequenceNumber;

        // timestamp = 160
        packet[4] = 0x00;
        packet[5] = 0x00;
        packet[6] = 0x00;
        packet[7] = 0xA0;

        // SSRC = 0x12345678
        packet[8] = 0x12;
        packet[9] = 0x34;
        packet[10] = 0x56;
        packet[11] = 0x78;

        for (var i = 12; i < packet.Length; i++)
            packet[i] = (byte)(i & 0xFF);

        return packet;
    }

}
