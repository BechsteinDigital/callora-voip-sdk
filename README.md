# CalloraVoipSdk

[![CI](https://github.com/BechsteinDigital/CalloraVoipSdk/actions/workflows/ci.yml/badge.svg)](https://github.com/BechsteinDigital/CalloraVoipSdk/actions/workflows/ci.yml)
![Coverage](https://img.shields.io/badge/coverage-cobertura%20artifact-blue)

CalloraVoipSdk ist eine .NET-8-VoIP-SDK für PBX-, Contact-Center- und Voice-Automation-Szenarien.  
Die API ist DDD-orientiert aufgebaut und bietet eine stabile Facade über `VoipClient`.

## Status

Aktuell verfügbar (Stand im Repository):

- SIP-Grundfunktionen: Register, Invite/Dial, Accept, Hangup, Hold/Unhold
- Erweiterte Call-Steuerung: DTMF, Blind/Attended Transfer
- In-Dialog-Operationen: `INFO`, `OPTIONS`, `SUBSCRIBE`, `NOTIFY`
- Medienpfad: RTP-Sessions, Sender/Receiver, `MediaConnector` (inkl. Cross-Connect)
- Conference Rooms: `ConferenceManager` + `ConferenceRoom` mit PCM16-Mixing, Mute und Teilnehmer-Level
- Recording/Playback: WAV/MP3 + Built-in Payload-Transcoding fuer L16/PCMU/PCMA/G.722
- Audio-Geräte für Linux und Windows als separate Projekte
- Runtime Device Controls: Device-Hot-Switch, Input/Output Mute, Input/Output Volume, Format-Update
- Runtime Plugin-System: dynamische Modul-Exports mit `install/activate/deactivate/uninstall` ohne Prozessneustart
- Plugin-Katalog mit Mehrfach-Exports je Vertragstyp (mehrere Erweiterungen parallel moeglich)
- Umfangreiche RFC-orientierte Unit- und Compliance-Tests

## Architektur (DDD)

- `src/Core/Domain`: Entitäten, Value Objects, Zustände, Domain-Events
- `src/Core/Application`: Use-Cases und Orchestrierung (Calls, Lines, Media)
- `src/Core/Infrastructure`: SIP/RTP/SDP/Audio-Adaptionen
- `src/Client`: öffentliche Facade und DX-Manager (`VoipClient`, Convenience APIs, DI)
- `src/Host/PluginContracts`: host-zentrierte Kernel-Vertraege fuer Plugin-Lifecycle und Runtime-Entrypoints
- `src/Plugins/Voip`: telephony-orientiertes `voip`-Plugin (Engine-Export)

## Plattformbegriffe

- `Admin UI`: Betreiber-/Backoffice-Oberflaeche.
- `Workspace UI`: Nutzer-/Agenten-Oberflaeche.

Der Begriff `Storefront` wird fuer CalloraVoipSdk nicht verwendet.

## Plattform-Start (Engine + Host + Plugins)

Minimaler Startstack:

- `Engine`: dieses Repository (`VoipClient`, SIP/RTP/RTCP/SRTP, Media)
- `Host Backend`: ASP.NET Core API + PostgreSQL + Redis + OpenTelemetry
- `Admin UI`: Betreiber-/Backoffice-Flaechen
- `Workspace UI`: Agenten-/Nutzerflaechen
- `Plugins`: runtime-faehig installierbar/aktivierbar ohne Neustart

Details: `docs/portal/architecture/platform-bootstrap.md`

## API-Sichtbarkeit

- Für SDK-Nutzer ist `VoipClient` der zentrale Einstiegspunkt.
- `Infrastructure`-Typen gelten als interne Implementierungsdetails und sind kein stabiler Integrationsvertrag.
- `Application`-Typen werden nur dort sichtbar gehalten, wo sie für typische SDK-Nutzung zwingend nötig sind.

## Versionierung

- Wir folgen Semantic Versioning (`MAJOR.MINOR.PATCH`).
- Aktuelle Pre-Release-Linie: `0.9.x`; `1.0.0` ist das erste stabile Public Release.
- Öffentliche API-Änderungen werden über Snapshot-Tests abgesichert (`tests/CalloraVoipSdk.Core.Tests/PublicApi.approved.txt`).
- Deprecations laufen über `[Obsolete(...)]` vor einer Entfernung.
- Consumer-relevante Änderungen stehen in [`CHANGELOG.md`](CHANGELOG.md).
- Details: [`docs/SEMVER_POLICY.md`](docs/SEMVER_POLICY.md).

## Voraussetzungen

- .NET SDK 8.0+
- SIP-Account/PBX-Zugangsdaten
- Für echte Audio-I/O auf Linux: `CalloraVoipSdk.Audio.Linux` (PortAudio-basiert)
- Für echte Audio-I/O auf Windows: `CalloraVoipSdk.Audio.Windows` (NAudio-basiert)

## Build, Test, Demo

```bash
dotnet restore CalloraVoipSdk.sln
dotnet build CalloraVoipSdk.sln
dotnet test CalloraVoipSdk.sln
dotnet run --project samples/CalloraVoipSdk.Sample.BasicCalling/CalloraVoipSdk.Sample.BasicCalling.csproj
dotnet run --project samples/CalloraVoipSdk.Sample.Conference/CalloraVoipSdk.Sample.Conference.csproj
dotnet run --project samples/CalloraVoipSdk.Sample.RealtimeBridge/CalloraVoipSdk.Sample.RealtimeBridge.csproj
dotnet run --project perf/CalloraVoipSdk.Conferencing.Performance/CalloraVoipSdk.Conferencing.Performance.csproj
```

## Einbindung ins eigene Projekt

Per NuGet (Feed-Konfiguration vorausgesetzt):

```bash
dotnet add package CalloraVoipSdk.Core
dotnet add package CalloraVoipSdk.Audio.Windows   # Windows
dotnet add package CalloraVoipSdk.Audio.Linux     # Linux
```

Alternativ per `ProjectReference` (lokale Entwicklung):

```xml
<ItemGroup>
  <ProjectReference Include="..\voip\src\Client\CalloraVoipSdk.Client.csproj" />
  <ProjectReference Include="..\voip\src\Core\CalloraVoipSdk.Core.csproj" />
  <!-- Optional je nach Plattform -->
  <ProjectReference Include="..\voip\src\Audio\Linux\CalloraVoipSdk.Audio.Linux.csproj" />
  <!-- oder -->
  <ProjectReference Include="..\voip\src\Audio\Windows\CalloraVoipSdk.Audio.Windows.csproj" />
</ItemGroup>
```

## Runtime Plugins (ohne Neustart)

```csharp
// Voraussetzung: Host wiring muss ICalloraPluginRuntime bereitstellen (CalloraVoipSdk.Hosting).
var install = await client.ModuleManager.InstallAsync(
    "/opt/voipsdk/plugins/Acme.PlaybackPlugin.dll",
    entryTypeName: "Acme.Playback.PluginEntry");
if (!install.IsSuccess || install.Plugin is null)
    throw new InvalidOperationException(install.Message);

var activate = await client.ModuleManager.ActivateAsync(install.Plugin.PluginId);
if (!activate.IsSuccess)
    throw new InvalidOperationException(activate.Message);

// Plugin-Module sind danach sofort ueber die bestehenden Facades wirksam
// (z. B. client.PlaybackManager / client.ConferenceManager), ohne Neustart.

await client.ModuleManager.DeactivateAsync(install.Plugin.PluginId);
await client.ModuleManager.UninstallAsync(install.Plugin.PluginId);
```

Plugin-Paketmetadaten:

- Plugins liefern eine `registry.json` neben der Plugin-DLL (Composer-artiges Paketmanifest).
- Der Host liest diese Datei bei `/api/plugins/install` automatisch.
- Wenn `entryTypeName` im Request fehlt, wird er aus `registry.json` uebernommen.

## Host Backend API (erste Instanz)

Projekt:

- `src/Host/Backend/CalloraVoipSdk.Host.Backend.csproj`

Swagger/OpenAPI:

- UI: `http://localhost:5000/swagger`
- JSON: `http://localhost:5000/swagger/v1/swagger.json`

Start:

```bash
dotnet run --project src/Host/Backend/CalloraVoipSdk.Host.Backend.csproj
```

Authentifizierung:

- Header: `X-CalloraVoipSdk-Api-Key`
- Key-Quelle: `BackendHost:ApiKeys` in `src/Host/Backend/appsettings.json`
- Ohne gueltigen API-Key sind alle `/api/plugins/*` Endpunkte gesperrt.

Persistenz (lokal, Phase 1):

- SQLite-Datei: `BackendHost:DatabasePath` (Standard: `plugins/host.db`)
- Entity-Registry: installierter Plugin-Zustand (`Installed/Active/Inactive/Uninstalled`)
- Audit-Log: append-only Lifecycle-Audit in DB

Erste Endpunkte:

- `GET /health`
- `GET /api/plugins`
- `GET /api/plugins/installed`
- `GET /api/plugins/audit`
- `POST /api/plugins/install`
- `POST /api/plugins/install/nuget`
- `POST /api/plugins/{pluginId}/activate`
- `POST /api/plugins/{pluginId}/deactivate`
- `DELETE /api/plugins/{pluginId}`

NuGet-Install (lokaler Cache, Shopware-aehnlicher zweiter Pfad):

```bash
curl -X POST http://localhost:5000/api/plugins/install/nuget \
  -H "X-CalloraVoipSdk-Api-Key: callora-local-dev-key-change-me" \
  -H "Content-Type: application/json" \
  -d '{
    "packageId": "acme.callora.voice-plugin",
    "packageVersion": "1.2.3",
    "assemblyFileName": "Acme.CalloraVoipSdk.VoicePlugin.dll"
  }'
```

## Quickstart: Happy Path (Convenience)

```csharp
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Domain.Calls;
using CalloraVoipSdk.Core.Domain.Lines;
using CalloraVoipSdk;

using var loggerFactory = LoggerFactory.Create(b => b
    .AddConsole()
    .SetMinimumLevel(LogLevel.Information));

using var client = new VoipClient(new SdkConfiguration
{
    LoggerFactory = loggerFactory,
    UserAgent = "MySoftphone/1.0",
    MaxConcurrentCallsPerLine = 4
});

var connectResult = await client.ConnectAsync(
    new SipAccount
    {
        Username = "1001",
        Password = "secret",
        SipServer = "pbx.example.com",
        DisplayName = "Agent 1001",
        Transport = SipTransport.Tls
    },
    new ConnectOptions
    {
        Timeout = TimeSpan.FromSeconds(15),
        FailFastOnRegistrationFailed = true
    });

if (!connectResult.IsSuccess || connectResult.Line is null)
    throw new InvalidOperationException($"Connect fehlgeschlagen: {connectResult.Status}");

var line = connectResult.Line;

var dialResult = await client.DialAndWaitUntilConnectedAsync(
    line,
    "sip:1002@pbx.example.com",
    new DialWaitOptions
    {
        ConnectTimeout = TimeSpan.FromSeconds(30),
        HangupOnTimeout = true,
        HangupOnCancellation = true
    });

if (!dialResult.IsSuccess || dialResult.Call is null)
    throw new InvalidOperationException($"Dial fehlgeschlagen: {dialResult.Status}");

var call = dialResult.Call;
await client.AttachDefaultAudioAsync(call);

// Runtime audio controls (works for configured SDK audio device).
var inDevices = client.GetAvailableInputAudioDevices();
var outDevices = client.GetAvailableOutputAudioDevices();

if (inDevices.Count > 1)
    client.SwitchAudioInputDevice(inDevices[1].Id);
if (outDevices.Count > 1)
    client.SwitchAudioOutputDevice(outDevices[1].Id);
client.SetAudioInputVolume(0.8f);
client.SetAudioOutputVolume(1.1f);
client.SetAudioInputMuted(false);
client.SetAudioOutputMuted(false);
client.UpdateAudioFormat(new AudioDeviceFormat
{
    SampleRate = 16000,
    BitsPerSample = 16,
    Channels = 1
});

await call.SendDtmfAsync(new DtmfTone('5'));
await call.HoldAsync();
await call.UnholdAsync();
await call.HangupAsync();
```

## Erweiterter Event-Flow (für Power-User, unverändert)

```csharp
var line = client.Lines.Register(account);

line.StateChanged += (_, e) =>
    Console.WriteLine($"Line: {e.OldState} -> {e.NewState}");

var call = await line.DialAsync("sip:1002@pbx.example.com");
call.StateChanged += (_, e) =>
    Console.WriteLine($"Call {e.Call.CallId}: {e.OldState} -> {e.NewState}");
```

## Beispiel 2: Inbound Call annehmen, ablehnen oder umleiten

```csharp
using var subscription = client.OnIncomingCall(async call =>
{
    Console.WriteLine($"Inbound von: {call.RemoteParty}");

    if (IsInLunchBreak())
    {
        await call.RejectAsync(486, "Busy Here");
        return;
    }

    if (ShouldForwardToQueue(call.RemoteParty))
    {
        var result = await call.RedirectAsync(["sip:queue@pbx.example.com"], statusCode: 302);
        Console.WriteLine($"Redirect: {result.Status}");
        return;
    }

    await call.AcceptAsync();
    await client.AttachDefaultAudioAsync(call);
});

static bool IsInLunchBreak() => false;
static bool ShouldForwardToQueue(string remoteParty) => false;
```

## Beispiel 3: Medienpfad manuell steuern (Advanced)

```csharp
using CalloraVoipSdk.Audio.Linux; // Auf Windows: CalloraVoipSdk.Audio.Windows + WindowsAudioDevice
using CalloraVoipSdk.Core.Application.Ports.Audio;
using CalloraVoipSdk.Core.Domain.Calls;

using var audioDevice = new LinuxAudioDevice();
using var receiver = client.Media.CreateReceiver();
using var sender = client.Media.CreateSender();

call.StateChanged += (_, e) =>
{
    if (e.NewState == CallState.Connected)
    {
        receiver.AttachToCall(call);
        sender.AttachToCall(call);

        var audioParameters = call.MediaParameters is { } mp
            ? AudioConnectionParameters.From(mp)
            : AudioConnectionParameters.Default;

        audioDevice.Connect(receiver, sender, audioParameters);

        if (audioDevice is IAudioDeviceRuntimeControl runtime)
        {
            runtime.SetInputMuted(false);
            runtime.SetOutputMuted(false);
            runtime.SetInputVolume(0.9f);
            runtime.SetOutputVolume(1.0f);
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

## Beispiel 4: Zwei Calls direkt gegeneinander bridgen

```csharp
// Voraussetzung: callA und callB sind verbunden.
using var aRx = client.Media.CreateReceiver();
using var aTx = client.Media.CreateSender();
using var bRx = client.Media.CreateReceiver();
using var bTx = client.Media.CreateSender();

aRx.AttachToCall(callA);
aTx.AttachToCall(callA);
bRx.AttachToCall(callB);
bTx.AttachToCall(callB);

using var bridge = client.Media.CreateConnector().CrossConnect(aRx, aTx, bRx, bTx);

// bridge.Dispose() trennt die Medienkopplung wieder.
```

## Beispiel 5: Conference Room mit drei Teilnehmern

```csharp
using CalloraVoipSdk.Modules;

var conference = client.ConferenceManager.Create();

await conference.AddParticipantAsync(callA);
await conference.AddParticipantAsync(callB);
await conference.AddParticipantAsync(callC);

// Teilnehmer B stummschalten
await conference.SetParticipantMuteAsync(callB.CallId, true);

// Teilnehmer C leiser in den Mix einspeisen
await conference.SetParticipantLevelAsync(callC.CallId, 0.6f);

// Teilnehmer entfernen
await conference.RemoveParticipantAsync(callB.CallId);

// Konferenz sauber schließen (Ressourcenfreigabe)
await conference.CloseAsync();
```

## Wichtige Hinweise für Produktion

- `VoipClient`, `IPhoneLine`, `ICall` sauber über Lifecycle steuern und am Ende disposen/unregistern
- Call-Aktionen zustandsabhängig ausführen (`Connected`, `Ringing`, `OnHold`)
- Bei hoher Last Event-Handler kurz halten und nicht-blockierend implementieren
- Audio-Provider als separate Plattform-Module bewusst auswählen

## Relevante Doku im Repo

- `docs/SDK_PRODUCT_MEMORY.md`
- `docs/SDK_COMPLETION_TODO.md`
- `docs/RFC_VOIP_SDK_COMPLIANCE.md`
- `docs/RFC3261_CHAPTER_STATUS.md`
