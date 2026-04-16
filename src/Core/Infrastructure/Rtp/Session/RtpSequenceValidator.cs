namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Session;

/// <summary>
/// Tracks receive-side sequence state for one SSRC and classifies each incoming
/// sequence number per the algorithm in RFC 3550 §A.1.
/// </summary>
internal sealed class RtpSequenceValidator
{
    // RFC 3550 §A.1 constants
    private const int MaxDropout    = 3000;  // max packets ahead before treating as restart
    private const int MaxMisorder   = 100;   // max packets behind before dropping as too old
    // MinSequential = 1: accept the first packet immediately.
    // RFC 3550 recommends 2 for unknown sources, but SIP calls negotiate the source via SDP,
    // so probation serves no purpose here. Dropping the first packet breaks stateful codecs
    // (G.722 ADPCM) because the decoder state diverges from the encoder state at packet 2.
    private const int MinSequential = 1;

    private ushort _maxSeq;       // highest sequence number seen
    private uint   _cycles;       // seq number cycles (extended seq = cycles + seq)
    private uint   _baseSeq;      // first seq seen
    private int    _probation;    // remaining packets needed to confirm source
    private bool   _initialized;

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Validates the incoming sequence number and updates internal state.
    /// Returns the classification of the packet.
    /// </summary>
    public RtpSequenceResult Validate(ushort seq)
    {
        if (!_initialized)
            return Initialize(seq);

        var udiff = (ushort)(seq - _maxSeq); // unsigned 16-bit difference

        if (udiff == 0)
            return RtpSequenceResult.Duplicate;

        if (udiff < MaxDropout)
        {
            // In order or only slightly ahead — accept
            if (seq < _maxSeq)
                _cycles += 0x10000; // sequence number wrapped
            _maxSeq = seq;

            if (_probation > 0)
            {
                _probation--;
                return _probation == 0
                    ? RtpSequenceResult.Valid
                    : RtpSequenceResult.Probation;
            }

            return RtpSequenceResult.Valid;
        }

        if (udiff <= 0xFFFF - MaxMisorder)
        {
            // Far out of order — could be a source restart; require re-probation.
            // The jump packet itself is not counted, so MinSequential more sequential
            // packets must arrive before the source is accepted again (RFC 3550 §A.1).
            _probation = MinSequential;
            _maxSeq    = seq;
            return RtpSequenceResult.SequenceJump;
        }

        // Within MAX_MISORDER behind max_seq — late/duplicate
        return RtpSequenceResult.TooLate;
    }

    /// <summary>Extended (48-bit) sequence number: cycles + current max seq.</summary>
    public ulong ExtendedMaxSeq => _cycles + _maxSeq;

    // -------------------------------------------------------------------------
    // Init
    // -------------------------------------------------------------------------

    private RtpSequenceResult Initialize(ushort seq)
    {
        _baseSeq     = seq;
        _maxSeq      = seq;
        _cycles      = 0;
        _probation   = MinSequential - 1;
        _initialized = true;

        return _probation == 0
            ? RtpSequenceResult.Valid
            : RtpSequenceResult.Probation;
    }
}
