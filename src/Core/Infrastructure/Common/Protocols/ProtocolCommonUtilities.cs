using System.Security.Cryptography;
using System.Text;

namespace CalloraVoipSdk.Core.Infrastructure.Common.Protocols;

/// <summary>
/// Shared low-level helpers for protocol parsing and header construction.
/// These helpers are intentionally protocol-agnostic and reusable across SIP/RTP/SRTP modules.
/// </summary>
internal static class ProtocolCommonUtilities
{
    /// <summary>
    /// Splits a comma-separated string while preserving commas inside quoted sections.
    /// </summary>
    public static IEnumerable<string> SplitCommaSeparatedRespectingQuotes(string value)
    {
        var current = new StringBuilder();
        var inQuotes = false;
        foreach (var ch in value)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                current.Append(ch);
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                var token = current.ToString().Trim();
                if (token.Length > 0) yield return token;
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        var last = current.ToString().Trim();
        if (last.Length > 0) yield return last;
    }

    /// <summary>
    /// Returns true when a comma-separated token list contains a specific token.
    /// </summary>
    public static bool ContainsToken(string value, string token)
    {
        foreach (var candidate in value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var normalized = candidate.Trim().Trim('"');
            if (normalized.Equals(token, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Computes a lowercase hexadecimal MD5 digest string.
    /// </summary>
    public static string Md5HexLower(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes lowercase hexadecimal hash text for supported digest algorithms.
    /// </summary>
    public static bool TryHashHexLower(
        string value,
        string algorithm,
        out string hashHexLower)
    {
        hashHexLower = string.Empty;
        var bytes = Encoding.UTF8.GetBytes(value);
        var normalized = algorithm.Trim().ToUpperInvariant();
        byte[] hash = normalized switch
        {
            "MD5" => MD5.HashData(bytes),
            "SHA-256" => SHA256.HashData(bytes),
            "SHA-512-256" => TryComputeHash(bytes, "SHA-512/256"),
            "SHA-512/256" => TryComputeHash(bytes, "SHA-512/256"),
            _ => Array.Empty<byte>()
        };
        if (hash.Length == 0)
            return false;

        hashHexLower = Convert.ToHexString(hash).ToLowerInvariant();
        return true;
    }

    /// <summary>
    /// Tries to compute a hash via platform algorithm name and returns empty array when unavailable.
    /// </summary>
    private static byte[] TryComputeHash(
        byte[] bytes,
        string algorithmName)
    {
        try
        {
            using var hash = IncrementalHash.CreateHash(new HashAlgorithmName(algorithmName));
            hash.AppendData(bytes);
            return hash.GetHashAndReset();
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// Escapes a value for inclusion in a quoted header parameter.
    /// </summary>
    public static string EscapeQuotedHeaderValue(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
             .Replace("\"", "\\\"", StringComparison.Ordinal);
}
