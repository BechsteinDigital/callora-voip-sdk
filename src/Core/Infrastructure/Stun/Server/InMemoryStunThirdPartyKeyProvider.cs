namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// In-memory implementation of <see cref="IStunThirdPartyKeyProvider"/>.
/// </summary>
internal sealed class InMemoryStunThirdPartyKeyProvider : IStunThirdPartyKeyProvider
{
    private readonly IReadOnlyDictionary<string, StunThirdPartyKeyMaterial> _keys;

    /// <summary>
    /// Builds a provider from key-id to key-material mappings.
    /// </summary>
    public InMemoryStunThirdPartyKeyProvider(
        IReadOnlyDictionary<string, StunThirdPartyKeyMaterial> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        if (keys.Count == 0)
            throw new ArgumentException("At least one third-party key is required.", nameof(keys));

        _keys = keys;
    }

    /// <inheritdoc />
    public bool TryGetKeyMaterial(string keyId, out StunThirdPartyKeyMaterial keyMaterial)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyId);
        return _keys.TryGetValue(keyId, out keyMaterial!);
    }
}
