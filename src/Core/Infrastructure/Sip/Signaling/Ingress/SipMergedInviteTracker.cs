using System.Collections.Concurrent;
using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Tracks out-of-dialog INVITE identity tuples to detect merged requests per RFC3261 section 8.2.2.2.
/// </summary>
internal sealed class SipMergedInviteTracker
{
    private static readonly TimeSpan EntryTtl = TimeSpan.FromMinutes(2);

    private readonly ConcurrentDictionary<string, DateTimeOffset> _seen =
        new(StringComparer.Ordinal);
    private int _registrations;

    /// <summary>
    /// Registers one candidate INVITE. Returns true when request is considered merged duplicate.
    /// </summary>
    public bool IsMergedInvite(SipRequest request)
    {
        if (!string.Equals(request.Method, "INVITE", StringComparison.Ordinal))
            return false;
        if (!string.IsNullOrWhiteSpace(SipProtocol.ExtractTag(request.Header("To"))))
            return false;

        if (!TryBuildMergeKey(request, out var key))
            return false;

        var now = DateTimeOffset.UtcNow;
        if (_seen.TryGetValue(key, out var existingSeenAt))
        {
            if (now - existingSeenAt <= EntryTtl)
                return true;
            _seen[key] = now;
            return false;
        }

        _seen[key] = now;
        var registrationCount = Interlocked.Increment(ref _registrations);
        if (registrationCount % 64 == 0)
            PruneExpired(now);

        return false;
    }

    /// <summary>
    /// Prunes expired merge keys to keep bounded memory.
    /// </summary>
    private void PruneExpired(DateTimeOffset now)
    {
        foreach (var pair in _seen)
        {
            if (now - pair.Value <= EntryTtl)
                continue;
            _seen.TryRemove(pair.Key, out _);
        }
    }

    /// <summary>
    /// Builds one merge identifier from Call-ID, From tag and CSeq number.
    /// </summary>
    private static bool TryBuildMergeKey(SipRequest request, out string key)
    {
        key = string.Empty;
        var callId = request.Header("Call-ID");
        var fromTag = SipProtocol.ExtractTag(request.Header("From"));
        var cseqNumber = SipProtocol.ExtractCSeqNumber(request.Header("CSeq"));

        if (string.IsNullOrWhiteSpace(callId)
            || string.IsNullOrWhiteSpace(fromTag)
            || cseqNumber <= 0)
        {
            return false;
        }

        key = $"{callId}|{fromTag}|{cseqNumber}|INVITE";
        return true;
    }
}
