namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>Validated DTMF tone (0-9, *, #, A-D).</summary>
public readonly record struct DtmfTone
{
    public char Symbol { get; }
    public byte Code   { get; }

    public DtmfTone(char symbol)
    {
        Symbol = char.ToUpperInvariant(symbol);
        Code = Symbol switch
        {
            >= '0' and <= '9' => (byte)(Symbol - '0'),
            '*'               => 10,
            '#'               => 11,
            >= 'A' and <= 'D' => (byte)(Symbol - 'A' + 12),
            _ => throw new ArgumentException($"Invalid DTMF tone: '{symbol}'", nameof(symbol))
        };
    }

    public static DtmfTone FromCode(byte code) => new(code switch
    {
        <= 9              => (char)('0' + code),
        10                => '*',
        11                => '#',
        >= 12 and <= 15   => (char)('A' + code - 12),
        _                 => throw new ArgumentOutOfRangeException(nameof(code))
    });

    public override string ToString() => Symbol.ToString();
}
