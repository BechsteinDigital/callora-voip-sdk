namespace CalloraVoipSdk.Performance;

/// <summary>
/// Parsed command-line options for the perf runner.
/// </summary>
internal sealed class CliOptions
{
    private const double DefaultMaxRegressionPercent = 15.0;

    /// <summary>
    /// Optional file path for writing the current run as baseline JSON.
    /// </summary>
    public string? WriteBaselinePath { get; private set; }

    /// <summary>
    /// Optional file path to baseline JSON for regression-gate checks.
    /// </summary>
    public string? GateBaselinePath { get; private set; }

    /// <summary>
    /// Maximum allowed regression percentage per metric.
    /// </summary>
    public double MaxRegressionPercent { get; private set; } = DefaultMaxRegressionPercent;

    /// <summary>
    /// Parses command-line options.
    /// </summary>
    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--write-baseline":
                    options.WriteBaselinePath = ReadRequiredValue(args, ref i, "--write-baseline");
                    break;

                case "--gate":
                    options.GateBaselinePath = ReadRequiredValue(args, ref i, "--gate");
                    break;

                case "--max-regression-percent":
                    var parsed = ReadRequiredValue(args, ref i, "--max-regression-percent");
                    if (!double.TryParse(parsed, out var percent) || percent < 0)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(args),
                            parsed,
                            "--max-regression-percent must be a non-negative number.");
                    }

                    options.MaxRegressionPercent = percent;
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown argument '{args[i]}'. Supported: --write-baseline <path>, --gate <path>, --max-regression-percent <value>.");
            }
        }

        return options;
    }

    /// <summary>
    /// Reads the next required option value from command-line arguments.
    /// </summary>
    private static string ReadRequiredValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"Option '{option}' requires a value.");

        index++;
        return args[index];
    }
}
