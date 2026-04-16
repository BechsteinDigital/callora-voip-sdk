# Media & Audio

## Attach Default Audio (Convenience)

```csharp
// Uses the SDK's configured default audio device + receiver/sender wiring.
await client.AttachDefaultAudioAsync(call);
// Auto-detach happens on call termination.
```

## Runtime Device Controls (Public SDK API)

```csharp
// Works with the SDK-configured device (outside convenience attach as well).
var inputDevices = client.GetAvailableInputAudioDevices();
var outputDevices = client.GetAvailableOutputAudioDevices();

if (inputDevices.Count > 1)
    client.SwitchAudioInputDevice(inputDevices[1].Id);
if (outputDevices.Count > 1)
    client.SwitchAudioOutputDevice(outputDevices[1].Id);

client.SetAudioInputMuted(false);
client.SetAudioOutputMuted(false);
client.SetAudioInputVolume(0.8f);   // 0..2
client.SetAudioOutputVolume(1.1f);  // 0..2

client.UpdateAudioFormat(new AudioDeviceFormat
{
    SampleRate = 16000,
    BitsPerSample = 16,
    Channels = 1
});

var snapshot = client.GetAudioDeviceRuntimeSnapshot();
Console.WriteLine(
    $"Audio runtime: in={snapshot.InputDeviceId}, out={snapshot.OutputDeviceId}, " +
    $"muted(in/out)={snapshot.InputMuted}/{snapshot.OutputMuted}, " +
    $"vol(in/out)={snapshot.InputVolume:F2}/{snapshot.OutputVolume:F2}");
```

## Attach an Audio Device Manually (Advanced)

```csharp
using CalloraVoipSdk.Audio.Linux; // Windows: CalloraVoipSdk.Audio.Windows + WindowsAudioDevice
using CalloraVoipSdk.Core.Application.Ports.Audio;
using CalloraVoipSdk.Core.Domain.Calls;

using var audioDevice = new LinuxAudioDevice();
using var receiver    = client.Media.CreateReceiver();
using var sender      = client.Media.CreateSender();

call.StateChanged += (_, e) =>
{
    if (e.NewState == CallState.Connected)
    {
        receiver.AttachToCall(call);
        sender.AttachToCall(call);

        var audioParams = call.MediaParameters is { } mp
            ? AudioConnectionParameters.From(mp)
            : AudioConnectionParameters.Default;

        audioDevice.Connect(receiver, sender, audioParams);

        if (audioDevice is IAudioDeviceRuntimeControl runtime)
        {
            runtime.SetInputVolume(0.9f);
            runtime.SetOutputVolume(1.0f);
            runtime.SetInputMuted(false);
            runtime.SetOutputMuted(false);
        }
    }

    if (e.NewState == CallState.Terminated)
    {
        audioDevice.Disconnect();
        receiver.Detach();
        sender.Detach();
    }
};
```

## Bridge Two Calls (Media Cross-Connect)

Directly couple two call legs without involving the local audio device:

```csharp
using var aRx = client.Media.CreateReceiver();
using var aTx = client.Media.CreateSender();
using var bRx = client.Media.CreateReceiver();
using var bTx = client.Media.CreateSender();

aRx.AttachToCall(callA);
aTx.AttachToCall(callA);
bRx.AttachToCall(callB);
bTx.AttachToCall(callB);

using var bridge = client.Media.CreateConnector()
    .CrossConnect(aRx, aTx, bRx, bTx);

// bridge.Dispose() removes the media coupling
```

## Conference Room

```csharp
using CalloraVoipSdk.Modules;

var conference = client.ConferenceManager.Create();

var addResult = await conference.AddParticipantAsync(callA);
// Check addResult.Success for error handling in production
_ = await conference.AddParticipantAsync(callB);
_ = await conference.AddParticipantAsync(callC);

// Mute a participant
_ = await conference.SetParticipantMuteAsync(callB.CallId, isMuted: true);

// Adjust mix level (0.0 – 1.0)
_ = await conference.SetParticipantLevelAsync(callC.CallId, level: 0.6f);

// Remove one participant
_ = await conference.RemoveParticipantAsync(callB.CallId);

// Tear down the conference
_ = await conference.CloseAsync();
```

## RTCP Quality Metrics

```csharp
var quality = call.QualitySnapshot;
if (quality.RtcpActive)
{
    Console.WriteLine($"Packet loss: {quality.LocalReceivePacketLossPercent:F1}%");
    Console.WriteLine($"Round-trip:  {quality.RoundTripTimeMs} ms");
    Console.WriteLine($"Jitter:      {quality.LocalReceiveJitterMs} ms");
}
```

## Recording (Call und Conference)

```csharp
// Call recording (WAV)
await using var callRecording = await client.Media.StartCallRecordingAsync(
    call,
    new RecordingOptions
    {
        OutputDirectory = "recordings",
        FileNamePrefix = "call",
        Format = AudioFileFormat.Wav,
        RotateAfterBytes = 25 * 1024 * 1024 // 25 MB parts
    });

// Pause/Resume/Stop
await callRecording.PauseAsync();
await callRecording.ResumeAsync();
await callRecording.StopAsync();

// Conference recording (mixed bus)
await using var conferenceRecording = await client.Media.StartConferenceRecordingAsync(
    conference,
    new RecordingOptions
    {
        OutputDirectory = "recordings",
        FileNamePrefix = "conference",
        Format = AudioFileFormat.Wav,
        SkipSilence = true,
        SilenceThresholdPcm16 = 96
    });
```

## Playback (Call und Conference)

```csharp
// Inject WAV playback into an active call
await using var callPlayback = await client.Media.StartCallPlaybackAsync(
    call,
    new PlaybackRequest
    {
        FilePath = "prompts/welcome.wav",
        Format = AudioFileFormat.Wav,
        Options = new PlaybackOptions
        {
            StartPaused = false,
            Loop = false
        }
    });

// Broadcast WAV playback into an active conference
await using var conferencePlayback = await client.Media.StartConferencePlaybackAsync(
    conference,
    new PlaybackRequest
    {
        FilePath = "prompts/announcement.wav",
        Format = AudioFileFormat.Wav
    });
```

Hinweis:
- WAV Recording/Playback ist end-to-end verfuegbar.
- MP3 Recording/Playback ist ueber den MP3 Infrastructure-Adapter verfuegbar (Encode/Decode ueber ffmpeg).
- Fuer bereits MP3-negotiated Calls nutzt die Session MP3-Passthrough ohne zusaetzliches Re-Encoding.
- Optional: Recordings koennen ueber `RecordingOptions.EncryptionProvider` (z. B. `AesGcmRecordingEncryptionProvider`) verschluesselt werden.
- Built-in Payload-Transcoding deckt L16/PCMU/PCMA/G.722 ab; weitere Codec-Familien koennen als zusaetzliche Adapter eingebunden werden.
