namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

/// <summary>
/// Stores repeated non-combinable SIP header rows without losing row boundaries.
/// </summary>
internal static class SipHeaderValueStorage
{
    private const char RowSeparator = '\n';

    /// <summary>
    /// Appends one header row value to an already stored value payload.
    /// </summary>
    public static string AppendRow(string existingStoredValue, string nextRowValue)
    {
        if (string.IsNullOrWhiteSpace(existingStoredValue))
            return nextRowValue;
        if (string.IsNullOrWhiteSpace(nextRowValue))
            return existingStoredValue;
        return $"{existingStoredValue}{RowSeparator}{nextRowValue}";
    }

    /// <summary>
    /// Splits a stored header value into individual row values.
    /// </summary>
    public static IReadOnlyList<string> SplitRows(string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
            return Array.Empty<string>();

        var rows = storedValue
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split(RowSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return rows;
    }

    /// <summary>
    /// Returns first stored row value or null when absent.
    /// </summary>
    public static string? FirstRow(string? storedValue)
    {
        var rows = SplitRows(storedValue);
        return rows.Count == 0 ? null : rows[0];
    }
}

