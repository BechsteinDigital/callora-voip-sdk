# Changelog

## Unreleased

- ICE State Machine (CORE-006) — in progress
- RTCP quality metrics (CORE-008) — in progress
- RFC 4733 DTMF / telephone-event (CORE-009) — in progress
- Recording/Playback (CORE-003) — done (WAV e2e + MP3 encode/decode)
- Runtime Device Controls (CORE-004) — done (runtime hot-switch + mute/volume + format update)
- Conferencing perf runner (N=2/4/8 mix benchmark + optional baseline gate) — done
- Recording options extended: optional AES-GCM file encryption + silence skipping (VAD-lite) — done
- Recording/Playback transcoding now includes G.722 (PT=9) via built-in codec adapter — done
- Conference hotpath mixing/allocation hardening: ArrayPool end-to-end + SIMD path + RFC3389 CN fallback for silent target frames — done
- Runtime plugin lifecycle added: install/activate/deactivate/uninstall + persisted plugin registry + dynamic module facade switching without restart — done

## 0.1.0 — Initial Release

- SIP signaling fundamentals (Register, Dial, Accept, Hangup, Hold/Unhold, Transfer)
- RTP/SRTP media transport with adaptive jitter buffer
- Conference rooms with PCM16 mixing
- Media cross-connect (bridge two calls)
- Linux (PortAudio) and Windows (NAudio) audio device support
