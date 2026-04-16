namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Strongly-typed, immutable identifier for a single call instance.
/// </summary>
public readonly record struct CallId(Guid Value)
{
    /// <summary>Creates a new <see cref="CallId"/> backed by a freshly generated <see cref="Guid"/>.</summary>
    /// <returns>A unique <see cref="CallId"/>.</returns>
    public static CallId New() => new(Guid.NewGuid());

    /// <summary>Returns the underlying GUID as a string.</summary>
    public override string ToString() => Value.ToString();
}
