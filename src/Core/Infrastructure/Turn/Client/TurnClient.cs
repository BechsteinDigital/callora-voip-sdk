using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;
using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;
using CalloraVoipSdk.Core.Infrastructure.Stun.Wire;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;
using CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// TURN client built as an isolated protocol module on top of the STUN wire codec/transport.
/// <para>
/// The TURN business orchestration (allocation lifecycle policy, media routing, retries across pools)
/// is intentionally out of scope here; this class provides protocol-accurate building blocks for
/// Allocate/Refresh/CreatePermission/ChannelBind/Send transactions.
/// </para>
/// </summary>
internal sealed class TurnClient : ITurnClient
{
    private readonly IStunMessageCodec _codec;
    private readonly ILogger<TurnClient> _logger;
    private readonly TurnTcpDataConnectionFactory _tcpDataConnectionFactory;

    /// <summary>Initial UDP retransmission timeout in milliseconds.</summary>
    private const int InitialRtoMs = 500;

    /// <summary>Maximum UDP retransmission timeout in milliseconds.</summary>
    private const int MaxRtoMs = 16_000;

    /// <summary>Total UDP transmission attempts (1 send + 6 retransmissions).</summary>
    private const int MaxAttempts = 7;

    /// <summary>Single response timeout in milliseconds for TCP/TLS requests.</summary>
    private const int ReliableResponseTimeoutMs = 8_000;

    /// <summary>
    /// Creates a TURN client with the required wire codec and logger.
    /// </summary>
    public TurnClient(IStunMessageCodec codec, ILogger<TurnClient> logger)
    {
        ArgumentNullException.ThrowIfNull(codec);
        ArgumentNullException.ThrowIfNull(logger);
        _codec = codec;
        _logger = logger;
        _tcpDataConnectionFactory = new TurnTcpDataConnectionFactory(codec, logger);
    }

    /// <inheritdoc />
    public async Task<TurnAllocateResult> AllocateAsync(
        IPEndPoint serverEndPoint,
        StunCredentials? credentials,
        TurnAllocateOptions? options = null,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);
        options ??= new TurnAllocateOptions();

        var (response, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.Allocate,
                transactionId => BuildAllocateAttributes(options, transactionId),
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        var relayed = TurnAttributeMapper.DecodeXorRelayedAddress(response)?.EndPoint;
        if (relayed is null)
            throw new TurnException("TURN Allocate success response is missing XOR-RELAYED-ADDRESS.");

        var mapped = response.Attributes.OfType<XorMappedAddressAttribute>().FirstOrDefault()?.EndPoint
                     ?? response.Attributes.OfType<MappedAddressAttribute>().FirstOrDefault()?.EndPoint;

        var lifetime = TurnAttributeMapper.DecodeLifetime(response)?.Seconds ?? 0;
        var mobilityTicket = TurnAttributeMapper.DecodeMobilityTicket(response)?.Ticket.ToArray();

        return new TurnAllocateResult
        {
            RelayedEndPoint = relayed,
            MappedEndPoint = mapped,
            LifetimeSeconds = lifetime,
            EffectiveCredentials = effectiveCredentials,
            MobilityTicket = mobilityTicket
        };
    }

    /// <inheritdoc />
    public async Task<TurnRefreshResult> RefreshAsync(
        IPEndPoint serverEndPoint,
        StunCredentials? credentials,
        uint? requestedLifetimeSeconds = null,
        ReadOnlyMemory<byte>? mobilityTicket = null,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);

        var (response, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.Refresh,
                _ =>
                {
                    if (!requestedLifetimeSeconds.HasValue)
                    {
                        if (!mobilityTicket.HasValue)
                            return Array.Empty<StunAttribute>();

                        return
                        [
                            TurnAttributeMapper.Encode(new TurnMobilityTicketAttribute
                            {
                                Ticket = mobilityTicket.Value.ToArray()
                            })
                        ];
                    }
                    var attributes = new List<StunAttribute>
                    {
                        TurnAttributeMapper.Encode(new TurnLifetimeAttribute
                        {
                            Seconds = requestedLifetimeSeconds.Value
                        })
                    };
                    if (mobilityTicket.HasValue)
                    {
                        attributes.Add(
                            TurnAttributeMapper.Encode(new TurnMobilityTicketAttribute
                            {
                                Ticket = mobilityTicket.Value.ToArray()
                            }));
                    }
                    return attributes;
                },
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        return new TurnRefreshResult
        {
            LifetimeSeconds = TurnAttributeMapper.DecodeLifetime(response)?.Seconds ?? 0,
            EffectiveCredentials = effectiveCredentials,
            MobilityTicket = TurnAttributeMapper.DecodeMobilityTicket(response)?.Ticket.ToArray()
        };
    }

    /// <inheritdoc />
    public async Task<StunCredentials?> CreatePermissionAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);
        ArgumentNullException.ThrowIfNull(peerEndPoint);

        var (_, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.CreatePermission,
                transactionId =>
                [
                    TurnAttributeMapper.Encode(
                        new TurnXorPeerAddressAttribute { EndPoint = peerEndPoint },
                        transactionId)
                ],
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        return effectiveCredentials;
    }

    /// <inheritdoc />
    public async Task<StunCredentials?> ChannelBindAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        ushort channelNumber,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);
        ArgumentNullException.ThrowIfNull(peerEndPoint);

        if (channelNumber < 0x4000 || channelNumber > 0x7FFF)
            throw new ArgumentOutOfRangeException(nameof(channelNumber), "TURN channel numbers must be in range 0x4000..0x7FFF.");

        var (_, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.ChannelBind,
                transactionId =>
                [
                    TurnAttributeMapper.Encode(new TurnChannelNumberAttribute { ChannelNumber = channelNumber }),
                    TurnAttributeMapper.Encode(
                        new TurnXorPeerAddressAttribute { EndPoint = peerEndPoint },
                        transactionId)
                ],
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        return effectiveCredentials;
    }

    /// <inheritdoc />
    public async Task<TurnConnectResult> ConnectAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);
        ArgumentNullException.ThrowIfNull(peerEndPoint);

        var (response, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.Connect,
                transactionId =>
                [
                    TurnAttributeMapper.Encode(
                        new TurnXorPeerAddressAttribute { EndPoint = peerEndPoint },
                        transactionId)
                ],
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        var connectionId = TurnAttributeMapper.DecodeConnectionId(response)?.ConnectionId;
        if (!connectionId.HasValue)
            throw new TurnException("TURN CONNECT success response is missing CONNECTION-ID.");

        return new TurnConnectResult
        {
            ConnectionId = connectionId.Value,
            EffectiveCredentials = effectiveCredentials
        };
    }

    /// <inheritdoc />
    public async Task<StunCredentials?> ConnectionBindAsync(
        IPEndPoint serverEndPoint,
        uint connectionId,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);

        var (_, effectiveCredentials) = await ExecuteWithAuthAsync(
                serverEndPoint,
                TurnMessageMethod.ConnectionBind,
                _ =>
                [
                    TurnAttributeMapper.Encode(new TurnConnectionIdAttribute
                    {
                        ConnectionId = connectionId
                    })
                ],
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        return effectiveCredentials;
    }

    /// <inheritdoc />
    public Task<TurnTcpDataConnection> OpenTcpDataConnectionAsync(
        IPEndPoint serverEndPoint,
        uint connectionId,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Tcp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);

        return _tcpDataConnectionFactory.OpenAsync(
            serverEndPoint,
            connectionId,
            credentials,
            transport,
            tlsTargetHost,
            tlsRemoteCertificateValidationCallback,
            ct);
    }

    /// <inheritdoc />
    public async Task SendIndicationAsync(
        IPEndPoint serverEndPoint,
        IPEndPoint peerEndPoint,
        ReadOnlyMemory<byte> payload,
        StunCredentials? credentials,
        TurnTransport transport = TurnTransport.Udp,
        string? tlsTargetHost = null,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serverEndPoint);
        ArgumentNullException.ThrowIfNull(peerEndPoint);

        await SendIndicationCoreAsync(
                serverEndPoint,
                TurnMessageMethod.Send,
                transactionId =>
                [
                    TurnAttributeMapper.Encode(
                        new TurnXorPeerAddressAttribute { EndPoint = peerEndPoint },
                        transactionId),
                    TurnAttributeMapper.Encode(new TurnDataAttribute { Value = payload.ToArray() })
                ],
                credentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);
    }

    // ── Auth flow ────────────────────────────────────────────────────────────

    private async Task<(StunMessage Response, StunCredentials? EffectiveCredentials)> ExecuteWithAuthAsync(
        IPEndPoint serverEndPoint,
        TurnMessageMethod method,
        Func<byte[], IReadOnlyList<StunAttribute>> attributeFactory,
        StunCredentials? credentials,
        TurnTransport transport,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        if (credentials is null || !credentials.IsLongTerm)
        {
            var response = await ExecuteRequestCoreAsync(
                    serverEndPoint,
                    method,
                    attributeFactory,
                    credentials,
                    transport,
                    tlsTargetHost,
                    tlsRemoteCertificateValidationCallback,
                    ct)
                .ConfigureAwait(false);

            return (response, ApplyAuthUpdates(response, credentials));
        }

        var activeCredentials = credentials;

        // Long-term TURN flow starts unauthenticated when REALM/NONCE are not known yet.
        if (activeCredentials.Realm is null || activeCredentials.Nonce is null)
        {
            try
            {
                _ = await ExecuteRequestCoreAsync(
                        serverEndPoint,
                        method,
                        attributeFactory,
                        credentials: null,
                        transport,
                        tlsTargetHost,
                        tlsRemoteCertificateValidationCallback,
                        ct)
                    .ConfigureAwait(false);
            }
            catch (TurnChallengeException challenge) when (challenge.ErrorCode == 401
                                                           && challenge.Realm is not null
                                                           && challenge.Nonce is not null)
            {
                activeCredentials = activeCredentials.WithRealmAndNonce(challenge.Realm, challenge.Nonce);
            }
        }

        try
        {
            var response = await ExecuteRequestCoreAsync(
                    serverEndPoint,
                    method,
                    attributeFactory,
                    activeCredentials,
                    transport,
                    tlsTargetHost,
                    tlsRemoteCertificateValidationCallback,
                    ct)
                .ConfigureAwait(false);

            return (response, ApplyAuthUpdates(response, activeCredentials));
        }
        catch (TurnChallengeException staleNonce) when (staleNonce.ErrorCode == 438 && staleNonce.Nonce is not null)
        {
            activeCredentials = activeCredentials.WithNonce(staleNonce.Nonce);
        }

        var finalResponse = await ExecuteRequestCoreAsync(
                serverEndPoint,
                method,
                attributeFactory,
                activeCredentials,
                transport,
                tlsTargetHost,
                tlsRemoteCertificateValidationCallback,
                ct)
            .ConfigureAwait(false);

        return (finalResponse, ApplyAuthUpdates(finalResponse, activeCredentials));
    }

    // ── Request transport ────────────────────────────────────────────────────

    private async Task<StunMessage> ExecuteRequestCoreAsync(
        IPEndPoint serverEndPoint,
        TurnMessageMethod method,
        Func<byte[], IReadOnlyList<StunAttribute>> attributeFactory,
        StunCredentials? credentials,
        TurnTransport transport,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        var transactionId = CreateTransactionId();
        var request = BuildMessage(
            StunMessageClass.Request,
            method,
            transactionId,
            attributeFactory(transactionId),
            credentials);

        var requestBytes = EncodeMessage(request, credentials);

        return transport switch
        {
            TurnTransport.Udp => await SendRequestUdpAsync(serverEndPoint, request, requestBytes, ct).ConfigureAwait(false),
            TurnTransport.Tcp => await SendRequestStreamAsync(
                    serverEndPoint,
                    request,
                    requestBytes,
                    tls: false,
                    tlsTargetHost,
                    tlsRemoteCertificateValidationCallback,
                    ct)
                .ConfigureAwait(false),
            TurnTransport.Tls => await SendRequestStreamAsync(
                    serverEndPoint,
                    request,
                    requestBytes,
                    tls: true,
                    tlsTargetHost,
                    tlsRemoteCertificateValidationCallback,
                    ct)
                .ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(transport), transport, "Unsupported TURN transport.")
        };
    }

    private async Task SendIndicationCoreAsync(
        IPEndPoint serverEndPoint,
        TurnMessageMethod method,
        Func<byte[], IReadOnlyList<StunAttribute>> attributeFactory,
        StunCredentials? credentials,
        TurnTransport transport,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        var transactionId = CreateTransactionId();
        var indication = BuildMessage(
            StunMessageClass.Indication,
            method,
            transactionId,
            attributeFactory(transactionId),
            credentials);

        var bytes = EncodeMessage(indication, credentials);

        switch (transport)
        {
            case TurnTransport.Udp:
                using (var udp = new UdpClient(serverEndPoint.AddressFamily))
                {
                    udp.Connect(serverEndPoint);
                    await udp.SendAsync(bytes, ct).ConfigureAwait(false);
                }
                break;

            case TurnTransport.Tcp:
            case TurnTransport.Tls:
                await SendIndicationStreamAsync(
                        serverEndPoint,
                        bytes,
                        tls: transport == TurnTransport.Tls,
                        tlsTargetHost,
                        tlsRemoteCertificateValidationCallback,
                        ct)
                    .ConfigureAwait(false);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(transport), transport, "Unsupported TURN transport.");
        }
    }

    private async Task<StunMessage> SendRequestUdpAsync(
        IPEndPoint serverEndPoint,
        StunMessage request,
        byte[] requestBytes,
        CancellationToken ct)
    {
        using var udp = new UdpClient(serverEndPoint.AddressFamily);
        udp.Connect(serverEndPoint);

        int rtoMs = InitialRtoMs;
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await udp.SendAsync(requestBytes, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TURN UDP send failed on attempt {Attempt}", attempt);
                throw new TurnException($"TURN UDP send failed: {ex.Message}", ex);
            }

            var response = await ReceiveMatchingUdpAsync(udp, request.TransactionId, rtoMs, ct).ConfigureAwait(false);
            if (response is null)
            {
                _logger.LogTrace("TURN attempt {Attempt} timed out after {Rto} ms", attempt, rtoMs);
                rtoMs = Math.Min(rtoMs * 2, MaxRtoMs);
                continue;
            }

            return ProcessResponse(response, request.MessageMethod);
        }

        throw new TurnException($"TURN server {serverEndPoint} did not respond after {MaxAttempts} attempts.");
    }

    private async Task<StunMessage> SendRequestStreamAsync(
        IPEndPoint serverEndPoint,
        StunMessage request,
        byte[] requestBytes,
        bool tls,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        using var tcp = new TcpClient(serverEndPoint.AddressFamily);

        try
        {
            await tcp.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "TURN {Transport} connect failed to {Server}", tls ? "TLS" : "TCP", serverEndPoint);
            throw new TurnException($"TURN connect failed: {ex.Message}", ex);
        }

        using var networkStream = tcp.GetStream();
        Stream stream = networkStream;

        if (tls)
        {
            var targetHost = string.IsNullOrWhiteSpace(tlsTargetHost) ? serverEndPoint.Address.ToString() : tlsTargetHost;
            var tlsStream = new SslStream(networkStream, leaveInnerStreamOpen: true, tlsRemoteCertificateValidationCallback);

            try
            {
                await tlsStream.AuthenticateAsClientAsync(
                        new SslClientAuthenticationOptions
                        {
                            TargetHost = targetHost,
                            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                        },
                        ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TURN TLS handshake failed for {Server}", serverEndPoint);
                throw new TurnException($"TURN TLS handshake failed: {ex.Message}", ex);
            }

            stream = tlsStream;
        }

        try
        {
            await stream.WriteAsync(requestBytes, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "TURN {Transport} send failed", tls ? "TLS" : "TCP");
            throw new TurnException($"TURN send failed: {ex.Message}", ex);
        }

        var response = await ReceiveMatchingStreamAsync(stream, request.TransactionId, ReliableResponseTimeoutMs, ct)
            .ConfigureAwait(false);
        if (response is null)
            throw new TurnException($"TURN server {serverEndPoint} did not respond on {(tls ? "TLS" : "TCP")}.");

        return ProcessResponse(response, request.MessageMethod);
    }

    private async Task SendIndicationStreamAsync(
        IPEndPoint serverEndPoint,
        byte[] bytes,
        bool tls,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        using var tcp = new TcpClient(serverEndPoint.AddressFamily);
        try
        {
            await tcp.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "TURN indication connect failed to {Server}", serverEndPoint);
            throw new TurnException($"TURN indication connect failed: {ex.Message}", ex);
        }

        using var networkStream = tcp.GetStream();
        Stream stream = networkStream;

        if (tls)
        {
            var targetHost = string.IsNullOrWhiteSpace(tlsTargetHost) ? serverEndPoint.Address.ToString() : tlsTargetHost;
            var tlsStream = new SslStream(networkStream, leaveInnerStreamOpen: true, tlsRemoteCertificateValidationCallback);

            try
            {
                await tlsStream.AuthenticateAsClientAsync(
                        new SslClientAuthenticationOptions
                        {
                            TargetHost = targetHost,
                            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                        },
                        ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TURN indication TLS handshake failed for {Server}", serverEndPoint);
                throw new TurnException($"TURN indication TLS handshake failed: {ex.Message}", ex);
            }

            stream = tlsStream;
        }

        try
        {
            await stream.WriteAsync(bytes, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "TURN indication send failed");
            throw new TurnException($"TURN indication send failed: {ex.Message}", ex);
        }
    }

    // ── Response processing ─────────────────────────────────────────────────

    private static StunMessage ProcessResponse(StunMessage response, StunMessageMethod expectedMethod)
    {
        if (response.MessageMethod != expectedMethod)
            throw new TurnException(
                $"TURN response method mismatch. Expected 0x{(ushort)expectedMethod:X4}, got 0x{(ushort)response.MessageMethod:X4}.");

        if (response.MessageClass == StunMessageClass.ErrorResponse)
        {
            var error = response.Attributes.OfType<ErrorCodeAttribute>().FirstOrDefault();
            int code = error?.Code ?? 0;
            string reason = string.IsNullOrWhiteSpace(error?.Reason) ? "(no reason)" : error!.Reason;

            if (code is 401 or 438)
            {
                var realm = response.Attributes.OfType<RealmAttribute>().FirstOrDefault()?.Value;
                var nonce = response.Attributes.OfType<NonceAttribute>().FirstOrDefault()?.Value;
                throw new TurnChallengeException(code, reason, realm, nonce);
            }

            throw new TurnException($"TURN error {code}: {reason}");
        }

        if (response.MessageClass != StunMessageClass.SuccessResponse)
            throw new TurnException($"TURN unexpected response class: {response.MessageClass}");

        return response;
    }

    private async Task<StunMessage?> ReceiveMatchingUdpAsync(
        UdpClient udp,
        byte[] transactionId,
        int timeoutMs,
        CancellationToken ct)
    {
        using var timeoutSource = new CancellationTokenSource(timeoutMs);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, ct);

        while (!linkedSource.Token.IsCancellationRequested)
        {
            UdpReceiveResult received;
            try
            {
                received = await udp.ReceiveAsync(linkedSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            var decoded = _codec.Decode(received.Buffer);
            if (decoded is not null && decoded.TransactionId.SequenceEqual(transactionId))
                return decoded;

            _logger.LogTrace("TURN discarded non-matching UDP packet ({Bytes} bytes)", received.Buffer.Length);
        }

        return null;
    }

    private async Task<StunMessage?> ReceiveMatchingStreamAsync(
        Stream stream,
        byte[] transactionId,
        int timeoutMs,
        CancellationToken ct)
    {
        using var timeoutSource = new CancellationTokenSource(timeoutMs);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, ct);

        while (!linkedSource.Token.IsCancellationRequested)
        {
            byte[]? raw;
            try
            {
                raw = await StunTcpFramer.ReadMessageAsync(stream, linkedSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TURN stream receive failed");
                throw new TurnException($"TURN stream receive failed: {ex.Message}", ex);
            }

            if (raw is null)
                return null;

            var decoded = _codec.Decode(raw);
            if (decoded is not null && decoded.TransactionId.SequenceEqual(transactionId))
                return decoded;

            _logger.LogTrace("TURN discarded non-matching stream packet ({Bytes} bytes)", raw.Length);
        }

        return null;
    }

    // ── Message construction ────────────────────────────────────────────────

    private static IReadOnlyList<StunAttribute> BuildAllocateAttributes(TurnAllocateOptions options, byte[] transactionId)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(transactionId);

        if (options.RequestedAddressFamily.HasValue && options.ReservationToken.HasValue)
        {
            throw new ArgumentException(
                "TURN Allocate must not include REQUESTED-ADDRESS-FAMILY together with RESERVATION-TOKEN.",
                nameof(options));
        }

        if (options.RequestedTransport == TurnRequestedTransportProtocol.Tcp
            && (options.DontFragment || options.ReserveEvenPort || options.ReservationToken.HasValue))
        {
            throw new ArgumentException(
                "TURN TCP allocations (RFC 6062) must not include DONT-FRAGMENT, EVEN-PORT, or RESERVATION-TOKEN.",
                nameof(options));
        }

        var attributes = new List<StunAttribute>
        {
            TurnAttributeMapper.Encode(new TurnRequestedTransportAttribute
            {
                Protocol = options.RequestedTransport
            })
        };

        if (options.RequestedAddressFamily.HasValue)
        {
            attributes.Add(TurnAttributeMapper.Encode(new TurnRequestedAddressFamilyAttribute
            {
                Family = options.RequestedAddressFamily.Value
            }));
        }

        if (options.LifetimeSeconds.HasValue)
        {
            attributes.Add(TurnAttributeMapper.Encode(new TurnLifetimeAttribute
            {
                Seconds = options.LifetimeSeconds.Value
            }));
        }

        if (options.ReserveEvenPort)
            attributes.Add(TurnAttributeMapper.Encode(new TurnEvenPortAttribute { ReserveNextPort = true }));

        if (options.DontFragment)
            attributes.Add(TurnAttributeMapper.Encode(new TurnDontFragmentAttribute()));

        if (options.ReservationToken.HasValue)
        {
            attributes.Add(TurnAttributeMapper.Encode(new TurnReservationTokenAttribute
            {
                Token = options.ReservationToken.Value
            }));
        }

        if (options.RequestMobilityTicket)
        {
            attributes.Add(TurnAttributeMapper.Encode(new TurnMobilityTicketAttribute
            {
                Ticket = Array.Empty<byte>()
            }));
        }

        return attributes;
    }

    private static StunMessage BuildMessage(
        StunMessageClass messageClass,
        TurnMessageMethod method,
        byte[] transactionId,
        IReadOnlyList<StunAttribute> methodAttributes,
        StunCredentials? credentials)
    {
        var attributes = new List<StunAttribute>();
        if (credentials is not null)
        {
            attributes.Add(new UsernameAttribute { Value = credentials.Username });

            if (credentials.IsLongTerm)
            {
                if (credentials.Realm is not null)
                    attributes.Add(new RealmAttribute { Value = credentials.Realm });
                if (credentials.Nonce is not null)
                    attributes.Add(new NonceAttribute { Value = credentials.Nonce });
            }
        }

        attributes.AddRange(methodAttributes);

        return new StunMessage
        {
            MessageClass = messageClass,
            MessageMethod = (StunMessageMethod)(ushort)method,
            TransactionId = transactionId,
            Attributes = attributes
        };
    }

    private byte[] EncodeMessage(StunMessage message, StunCredentials? credentials)
    {
        if (credentials is null)
            return _codec.Encode(message);

        return _codec.EncodeWithIntegrity(message, credentials.DeriveHmacKey(), addFingerprint: false);
    }

    private static byte[] CreateTransactionId()
    {
        var transactionId = new byte[StunWireConstants.TransactionIdLength];
        RandomNumberGenerator.Fill(transactionId);
        return transactionId;
    }

    private static StunCredentials? ApplyAuthUpdates(StunMessage response, StunCredentials? credentials)
    {
        if (credentials is null || !credentials.IsLongTerm)
            return credentials;

        var realm = response.Attributes.OfType<RealmAttribute>().FirstOrDefault()?.Value ?? credentials.Realm;
        var nonce = response.Attributes.OfType<NonceAttribute>().FirstOrDefault()?.Value ?? credentials.Nonce;

        if (realm is null || nonce is null)
            return credentials;

        return !string.Equals(realm, credentials.Realm, StringComparison.Ordinal)
               || !string.Equals(nonce, credentials.Nonce, StringComparison.Ordinal)
            ? credentials.WithRealmAndNonce(realm, nonce)
            : credentials;
    }
}
