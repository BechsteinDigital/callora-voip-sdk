namespace CalloraVoipSdk.Core.Domain.Lines;

/// <summary>Immutable, validated SIP address (e.g. sip:alice@example.com).</summary>
public readonly record struct SipAddress
{
    public string Value    { get; }
    public string User     { get; }
    public string Host     { get; }

    public SipAddress(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var normalised = value.StartsWith("sip:", StringComparison.OrdinalIgnoreCase) ||
                         value.StartsWith("sips:", StringComparison.OrdinalIgnoreCase)
            ? value
            : $"sip:{value}";

        var afterScheme = normalised[(normalised.IndexOf(':') + 1)..];
        var atIndex     = afterScheme.IndexOf('@');

        if (atIndex <= 0)
            throw new ArgumentException($"SIP address must contain user@host: '{value}'", nameof(value));

        User  = afterScheme[..atIndex];
        Host  = afterScheme[(atIndex + 1)..];
        Value = normalised;
    }

    public static SipAddress From(string username, string host) =>
        new($"sip:{username}@{host}");

    public override string ToString() => Value;
}
