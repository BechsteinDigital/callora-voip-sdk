using System.Net.WebSockets;
using System.Text;

namespace CalloraVoipSdk.Modules;

/// <summary>
/// SDK facade for reusable WebSocket features.
/// </summary>
public interface IWebSocketModule : IWebSocketAudioTransportModule
{
    /// <summary>
    /// Creates a generic websocket client connection.
    /// </summary>
    IWebSocketConnection CreateConnection(
        Uri endpoint,
        WebSocketClientOptions? options = null,
        IReadOnlyDictionary<string, string>? headers = null);
}

/// <summary>
/// SDK facade for reusable WebSocket audio-frame transports.
/// </summary>
public interface IWebSocketAudioTransportModule
{
    /// <summary>True when this module can be used in the current runtime context.</summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Creates a websocket transport bound to one endpoint.
    /// </summary>
    IAudioFrameStreamTransport Create(
        Uri endpoint,
        WebSocketAudioFrameTransportOptions? options = null,
        IReadOnlyDictionary<string, string>? headers = null);
}

/// <summary>
/// Generic websocket message kind.
/// </summary>
public enum WebSocketPayloadType
{
    Text = 0,
    Binary = 1,
}

/// <summary>
/// Generic websocket message payload.
/// </summary>
public readonly record struct WebSocketMessage(
    WebSocketPayloadType Type,
    ReadOnlyMemory<byte> Payload)
{
    /// <summary>
    /// Returns payload decoded as UTF-8 text.
    /// </summary>
    public string AsText() => Encoding.UTF8.GetString(Payload.Span);
}

/// <summary>
/// Generic websocket client connection contract.
/// </summary>
public interface IWebSocketConnection : IAsyncDisposable
{
    /// <summary>True while connection is open.</summary>
    bool IsConnected { get; }

    /// <summary>Connects websocket client.</summary>
    Task ConnectAsync(CancellationToken ct = default);

    /// <summary>Closes websocket client.</summary>
    Task CloseAsync(
        WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
        string? statusDescription = "closed",
        CancellationToken ct = default);

    /// <summary>Reads complete incoming messages.</summary>
    IAsyncEnumerable<WebSocketMessage> ReadMessagesAsync(CancellationToken ct = default);

    /// <summary>Sends one text message.</summary>
    ValueTask SendTextAsync(string message, CancellationToken ct = default);

    /// <summary>Sends one binary message.</summary>
    ValueTask SendBinaryAsync(ReadOnlyMemory<byte> payload, CancellationToken ct = default);

    /// <summary>Sends one message.</summary>
    ValueTask SendAsync(WebSocketMessage message, CancellationToken ct = default);
}

/// <summary>
/// Generic websocket client options.
/// </summary>
public class WebSocketClientOptions
{
    /// <summary>
    /// Optional websocket subprotocol.
    /// </summary>
    public string? SubProtocol { get; set; }

    /// <summary>
    /// Keepalive ping period.
    /// </summary>
    public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Receive buffer size in bytes.
    /// </summary>
    public int ReceiveBufferSizeBytes { get; set; } = 16 * 1024;

    /// <summary>
    /// Optional connect timeout.
    /// </summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Max accepted size of one complete incoming websocket message.
    /// </summary>
    public int MaxIncomingMessageBytes { get; set; } = 1024 * 1024;

    /// <summary>
    /// Optional provider called for each (re)connect to fetch dynamic request headers.
    /// </summary>
    public Func<CancellationToken, ValueTask<IReadOnlyDictionary<string, string>?>>? DynamicHeadersProvider { get; set; }
}

/// <summary>
/// WebSocket transport options.
/// </summary>
public sealed class WebSocketAudioFrameTransportOptions : WebSocketClientOptions
{
    /// <summary>
    /// Max accepted size of one decoded audio payload.
    /// </summary>
    public int MaxAudioPayloadBytes { get; set; } = 256 * 1024;
}
