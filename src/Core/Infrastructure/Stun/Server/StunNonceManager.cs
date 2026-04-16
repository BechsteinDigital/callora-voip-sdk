using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Thread-safe implementation of <see cref="IStunNonceManager"/>.
/// Generates 128-bit cryptographically random nonces encoded as Base64.
/// Each nonce is valid for a configurable duration (default: 5 minutes per RFC 5389 guidance).
/// <para>
/// Expired entries are lazily cleaned up each time a new nonce is generated.
/// </para>
/// </summary>
internal sealed class StunNonceManager : IStunNonceManager
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _nonces = new(StringComparer.Ordinal);
    private readonly TimeSpan _nonceTtl;
    private readonly TimeSpan _purgeInterval;
    private long _nextPurgeAtUtcTicks;

    /// <summary>Default nonce lifetime recommended by RFC 5389 §10.2.2.</summary>
    public static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

    /// <summary>Initialises with the default 5-minute nonce lifetime.</summary>
    public StunNonceManager() : this(DefaultTtl) { }

    /// <summary>Initialises with a custom nonce lifetime.</summary>
    /// <param name="nonceTtl">
    /// How long a nonce remains valid after issuance.
    /// Shorter values improve security; longer values reduce retransmit cost.
    /// </param>
    public StunNonceManager(TimeSpan nonceTtl)
    {
        if (nonceTtl <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(nonceTtl), "Nonce TTL must be positive.");

        _nonceTtl = nonceTtl;
        var halfTtlSeconds = (int)Math.Max(1, Math.Floor(nonceTtl.TotalSeconds / 2));
        _purgeInterval = TimeSpan.FromSeconds(Math.Clamp(halfTtlSeconds, 1, 60));
        _nextPurgeAtUtcTicks = DateTimeOffset.UtcNow.Add(_purgeInterval).UtcTicks;
    }

    /// <inheritdoc />
    public string GenerateNonce()
    {
        var now = DateTimeOffset.UtcNow;

        // 16 random bytes → 24-character Base64 string.
        var raw   = RandomNumberGenerator.GetBytes(16);
        var nonce = Convert.ToBase64String(raw);

        _nonces[nonce] = now + _nonceTtl;
        TryPurgeExpired(now);
        return nonce;
    }

    /// <inheritdoc />
    public bool IsNonceValid(string nonce)
    {
        if (!_nonces.TryGetValue(nonce, out var expiry))
            return false;

        if (DateTimeOffset.UtcNow <= expiry)
            return true;

        // Nonce is present but expired — remove it.
        _nonces.TryRemove(nonce, out _);
        return false;
    }

    /// <summary>
    /// Removes all expired nonces from the internal store.
    /// Called lazily during <see cref="GenerateNonce"/> to avoid unbounded growth.
    /// </summary>
    private void TryPurgeExpired(DateTimeOffset nowUtc)
    {
        var dueTicks = Volatile.Read(ref _nextPurgeAtUtcTicks);
        if (nowUtc.UtcTicks < dueTicks)
            return;

        var nextDue = nowUtc.Add(_purgeInterval).UtcTicks;
        if (Interlocked.CompareExchange(ref _nextPurgeAtUtcTicks, nextDue, dueTicks) != dueTicks)
            return;

        PurgeExpired(nowUtc);
    }

    private void PurgeExpired(DateTimeOffset nowUtc)
    {
        foreach (var (key, expiry) in _nonces)
        {
            if (nowUtc > expiry)
                _nonces.TryRemove(key, out _);
        }
    }
}
