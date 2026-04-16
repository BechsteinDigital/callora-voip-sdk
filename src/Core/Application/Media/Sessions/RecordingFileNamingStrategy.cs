using System.Text;
using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Builds deterministic recording output file names with part rotation suffixes.
/// </summary>
internal static class RecordingFileNamingStrategy
{
    /// <summary>
    /// Builds the next recording file path.
    /// </summary>
    public static string BuildFilePath(
        RecordingOptions options,
        string targetToken,
        int partIndex,
        DateTimeOffset utcNow)
    {
        var directory = options.OutputDirectory;
        var prefix = Sanitize(options.FileNamePrefix);
        var target = Sanitize(targetToken);
        var timestamp = options.IncludeUtcTimestamp
            ? $"-{utcNow:yyyyMMdd-HHmmssfff}"
            : string.Empty;

        var extension = options.Format == AudioFileFormat.Mp3 ? ".mp3" : ".wav";
        var fileName = $"{prefix}-{target}{timestamp}-part{partIndex:D4}{extension}";
        return Path.Combine(directory, fileName);
    }

    private static string Sanitize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "media";

        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value.Trim())
        {
            builder.Append(Array.IndexOf(invalid, ch) >= 0 || char.IsWhiteSpace(ch) ? '-' : ch);
        }

        return builder.Length == 0 ? "media" : builder.ToString();
    }
}
