using System.Net;
using System.Net.Security;
using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// TURN client operations built on top of the STUN wire transport.
/// </summary>
internal interface ITurnClient
{
    /// <summary>
    /// Performs TURN Allocate and returns the relayed endpoint information.
    /// </summary>
    Task<TurnAllocateResult> AllocateAsync(
        IPEndPoint serverEndPoint,
        StunCredentials? credentials,
        TurnAllocateOptions? options = null,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Performs TURN Refresh for an existing allocation.
    /// </summary>
    Task<TurnRefreshResult> RefreshAsync(
        IPEndPoint serverEndPoint,
        StunCredentials? credentials,
        uint? requestedLifetimeSeconds = null,
        ReadOnlyMemory<byte>? mobilityTicket = null,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Performs TURN CreatePermission for a peer endpoint.
    /// </summary>
    Task<StunCredentials?> CreatePermissionAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Performs TURN ChannelBind for a peer endpoint and channel number.
    /// </summary>
    Task<StunCredentials?> ChannelBindAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        ushort channelNumber,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Performs RFC 6062 CONNECT and returns the CONNECTION-ID.
    /// </summary>
    Task<TurnConnectResult> ConnectAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Performs RFC 6062 CONNECTION-BIND for an existing CONNECTION-ID.
    /// </summary>
    Task<StunCredentials?> ConnectionBindAsync(
        IPEndPoint serverEndPoint,
        uint connectionId,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Opens a persistent RFC 6062 data connection and performs CONNECTION-BIND on that same stream.
    /// </summary>
    Task<TurnTcpDataConnection> OpenTcpDataConnectionAsync(
        IPEndPoint serverEndPoint,
        uint connectionId,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Tcp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a TURN Send Indication containing application data.
    /// </summary>
    Task SendIndicationAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        ReadOnlyMemory<byte> payload,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default);
}
