---
_layout: landing
---

# CalloraVoipSdk

**Build your own voice product on a sovereign telephony core.**

European B2B voice runtime for teams building calling, dialer, contact center,
or voice AI products — with full technical control over telephony, media path,
and intelligent decision logic.

## Built for

- PBX and UC vendors
- Contact center software makers
- Dialer and campaign tools
- CRM/Sales automation with calling
- Voicebot and AI agent platforms
- Fraud, spam and scam detection systems

## Current Status — Phase 1 (active development)

**What works today:**

| Capability | Status |
|-----------|--------|
| SIP Register / Dial / Accept / Hangup | ✅ Production-ready |
| Hold / Unhold / Blind + Attended Transfer | ✅ Production-ready |
| RTP/SRTP media transport | ✅ Production-ready |
| Adaptive jitter buffer | ✅ Production-ready |
| Conference rooms (PCM16 mixing, mute, levels) | ✅ Production-ready |
| Media cross-connect / bridge | ✅ Production-ready |
| DTMF send/receive (RFC 4733) | ✅ Production-ready |
| RTCP quality metrics | ✅ Production-ready |
| Linux + Windows audio devices | ✅ Production-ready |
| Runtime device hot-switch + controls | ✅ Production-ready |
| Runtime plugin lifecycle (`install/activate/deactivate/uninstall`) | ✅ Production-ready |

**In progress / planned:**

| Capability | Status |
|-----------|--------|
| ICE / STUN / TURN (NAT traversal) | 🔧 In progress |
| Recording + Playback (WAV/MP3) | ✅ Production-ready |
| Backend/API for signed plugin marketplace + tenant entitlements | 📋 Roadmap |
| CalloraVoipSdk.Privacy module | 📋 Roadmap |
| CalloraVoipSdk.Risk module | 📋 Roadmap |
| CalloraVoipSdk.Intelligence module | 📋 Roadmap |

> NuGet packaging and first public release are planned once Phase 1 is complete.

## SDK Structure

**CalloraVoipSdk.Core** — Sovereign calling foundation

Clean DDD architecture: Domain → Application → Infrastructure → public `VoipClient` facade.
No vendor lock-in. Full protocol stack owned in-house (SIP, RTP, SRTP, SDP, STUN).

**Differentiating modules** *(roadmap)*

- **CalloraVoipSdk.Privacy** — Redaction, consent management, policy gates, audit trail
- **CalloraVoipSdk.Risk** — Spam/scam signals, call risk screening, PBX abuse prevention
- **CalloraVoipSdk.Intelligence** — AMD, sentiment, transcription, local model integration

## Quickstart

```csharp
using var client = new VoipClient(new SdkConfiguration
{
    LoggerFactory = loggerFactory,
    UserAgent = "MySoftphone/1.0"
});

var connectResult = await client.ConnectAsync(new SipAccount
{
    Username = "1001",
    Password = "secret",
    SipServer = "pbx.example.com"
});

if (!connectResult.IsSuccess || connectResult.Line is null)
    throw new InvalidOperationException($"Connect failed: {connectResult.Status}");

var dialResult = await client.DialAndWaitUntilConnectedAsync(
    connectResult.Line,
    "sip:1002@pbx.example.com");

if (!dialResult.IsSuccess || dialResult.Call is null)
    throw new InvalidOperationException($"Dial failed: {dialResult.Status}");

await client.AttachDefaultAudioAsync(dialResult.Call);
await dialResult.Call.HangupAsync();
```

[→ Getting Started Guide](guides/getting-started.md)
