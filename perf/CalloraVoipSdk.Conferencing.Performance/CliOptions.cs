namespace CalloraVoipSdk.Conferencing.Performance;

internal sealed class CliOptions
{
    private const double DefaultMaxRegressionPercent = 15.0;

    public string? WriteBaselinePath { get; private set; }
    public string? GateBaselinePath { get; private set; }
    public double MaxRegressionPercent { get; private set; } = DefaultMaxRegressionPercent;

    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--write-baseline":
                    options.WriteBaselinePath = RequireValue(args, ref i, "--write-baseline");
                    break;
                case "--gate-baseline":
                    options.GateBaselinePath = RequireValue(args, ref i, "--gate-baseline");
                    break;
                case "--max-regression-percent":
                    var value = RequireValue(args, ref i, "--max-regression-percent");
                    if (!double.TryParse(value, out var parsed) || parsed < 0)
                        throw new ArgumentException("Invalid value for --max-regression-percent.");
                    options.MaxRegressionPercent = parsed;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{args[i]}'.");
            }
        }

        return options;
    }

    private static string RequireValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"Missing value for {optionName}.");
        index++;
        return args[index];
    }
}
