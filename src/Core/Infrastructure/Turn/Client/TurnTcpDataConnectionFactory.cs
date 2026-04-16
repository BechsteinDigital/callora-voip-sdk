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
/// Opens persistent TURN TCP/TLS data connections and binds them using RFC 6062 CONNECTION-BIND.
/// </summary>
internal sealed class TurnTcpDataConnectionFactory
{
    private readonly IStunMessageCodec _codec;
    private readonly ILogger<TurnClient> _logger;

    /// <summary>
    /// Creates factory.
    /// </summary>
    public TurnTcpDataConnectionFactory(IStunMessageCodec codec, ILogger<TurnClient> logger)
    {
        ArgumentNullException.ThrowIfNull(codec);
        ArgumentNullException.ThrowIfNull(logger);
        _codec = codec;
        _logger = logger;
    }

    /// <summary>
    /// Opens one bound data connection and returns it ready for raw byte relay.
    /// </summary>
    public async Task<TurnTcpDataConnection> OpenAsync(
        IPEndPoint serverEndPoint,
        uint connectionId,
        StunCredentials? credentials,
        TurnTransport transport,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        if (transport == TurnTransport.Udp)
            throw new ArgumentException("RFC 6062 data connections require TCP or TLS transport.", nameof(transport));

        var tcp = new TcpClient(serverEndPoint.AddressFamily);
        try
        {
            await tcp.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port, ct).ConfigureAwait(false);
            var stream = await CreateClientStreamAsync(
                    tcp,
                    serverEndPoint,
                    transport == TurnTransport.Tls,
                    tlsTargetHost,
                    tlsRemoteCertificateValidationCallback,
                    ct)
                .ConfigureAwait(false);

            var effectiveCredentials = await SendConnectionBindWithAuthAsync(
                    stream,
                    connectionId,
                    credentials,
                    ct)
                .ConfigureAwait(false);

            return new TurnTcpDataConnection(tcp, stream, effectiveCredentials);
        }
        catch
        {
            tcp.Dispose();
            throw;
        }
    }

    private async Task<StunCredentials?> SendConnectionBindWithAuthAsync(
        Stream stream,
        uint connectionId,
        StunCredentials? credentials,
        CancellationToken ct)
    {
        if (credentials is null || !credentials.IsLongTerm)
        {
            _ = await SendConnectionBindAsync(stream, connectionId, credentials, ct).ConfigureAwait(false);
            return credentials;
        }

        var active = credentials;
        if (active.Realm is null || active.Nonce is null)
        {
            try
            {
                _ = await SendConnectionBindAsync(stream, connectionId, credentials: null, ct).ConfigureAwait(false);
            }
            catch (TurnChallengeException challenge) when (challenge.ErrorCode == 401
                                                           && challenge.Realm is not null
                                                           && challenge.Nonce is not null)
            {
                active = active.WithRealmAndNonce(challenge.Realm, challenge.Nonce);
            }
        }

        try
        {
            var response = await SendConnectionBindAsync(stream, connectionId, active, ct).ConfigureAwait(false);
            return ApplyAuthUpdates(response, active);
        }
        catch (TurnChallengeException stale) when (stale.ErrorCode == 438 && stale.Nonce is not null)
        {
            active = active.WithNonce(stale.Nonce);
        }

        var final = await SendConnectionBindAsync(stream, connectionId, active, ct).ConfigureAwait(false);
        return ApplyAuthUpdates(final, active);
    }

    private async Task<StunMessage> SendConnectionBindAsync(
        Stream stream,
        uint connectionId,
        StunCredentials? credentials,
        CancellationToken ct)
    {
        var txId = CreateTransactionId();
        var request = BuildConnectionBindRequest(txId, connectionId, credentials);
        var bytes = EncodeMessage(request, credentials);

        try
        {
            await stream.WriteAsync(bytes, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "TURN data connection send failed");
            throw new TurnException($"TURN data connection send failed: {ex.Message}", ex);
        }

        while (true)
        {
            TurnStreamFrame? frame;
            try
            {
                frame = await TurnStreamFramer.ReadFrameAsync(stream, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TURN data connection receive failed");
                throw new TurnException($"TURN data connection receive failed: {ex.Message}", ex);
            }

            if (frame is null)
                throw new TurnException("TURN data connection closed before CONNECTION-BIND completed.");

            if (frame.IsChannelData)
                continue;

            var response = _codec.Decode(frame.Payload);
            if (response is null || !response.TransactionId.SequenceEqual(txId))
                continue;

            return ProcessResponse(response);
        }
    }

    private static StunMessage ProcessResponse(StunMessage response)
    {
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

        if (response.MessageClass != StunMessageClass.SuccessResponse
            || response.MessageMethod != (StunMessageMethod)(ushort)TurnMessageMethod.ConnectionBind)
        {
            throw new TurnException("TURN unexpected response to CONNECTION-BIND.");
        }

        return response;
    }

    private static StunMessage BuildConnectionBindRequest(byte[] txId, uint connectionId, StunCredentials? credentials)
    {
        var attrs = new List<StunAttribute>();
        if (credentials is not null)
        {
            attrs.Add(new UsernameAttribute { Value = credentials.Username });
            if (credentials.IsLongTerm)
            {
                if (credentials.Realm is not null)
                    attrs.Add(new RealmAttribute { Value = credentials.Realm });
                if (credentials.Nonce is not null)
                    attrs.Add(new NonceAttribute { Value = credentials.Nonce });
            }
        }

        attrs.Add(TurnAttributeMapper.Encode(new TurnConnectionIdAttribute
        {
            ConnectionId = connectionId
        }));

        return new StunMessage
        {
            MessageClass = StunMessageClass.Request,
            MessageMethod = (StunMessageMethod)(ushort)TurnMessageMethod.ConnectionBind,
            TransactionId = txId,
            Attributes = attrs
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
        var txId = new byte[StunWireConstants.TransactionIdLength];
        RandomNumberGenerator.Fill(txId);
        return txId;
    }

    private static async Task<Stream> CreateClientStreamAsync(
        TcpClient tcp,
        IPEndPoint serverEndPoint,
        bool tls,
        string? tlsTargetHost,
        RemoteCertificateValidationCallback? tlsRemoteCertificateValidationCallback,
        CancellationToken ct)
    {
        var networkStream = tcp.GetStream();
        if (!tls)
            return networkStream;

        var targetHost = string.IsNullOrWhiteSpace(tlsTargetHost) ? serverEndPoint.Address.ToString() : tlsTargetHost;
        var tlsStream = new SslStream(networkStream, leaveInnerStreamOpen: true, tlsRemoteCertificateValidationCallback);

        await tlsStream.AuthenticateAsClientAsync(
                new SslClientAuthenticationOptions
                {
                    TargetHost = targetHost,
                    EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                },
                ct)
            .ConfigureAwait(false);

        return tlsStream;
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
