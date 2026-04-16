using System.Net;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

internal sealed record OrderedCandidatePair(
    CallIceCandidate LocalCandidate,
    CallIceCandidate RemoteCandidate,
    IPEndPoint LocalProbeEndPoint,
    IPEndPoint RemoteEndPoint,
    ulong PairPriority);
