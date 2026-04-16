namespace CalloraVoipSdk.Core.Domain.Lines;

public readonly record struct LineId(Guid Value)
{
    public static LineId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
