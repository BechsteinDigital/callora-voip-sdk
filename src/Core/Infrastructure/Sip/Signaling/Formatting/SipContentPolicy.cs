using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Validates SIP request body metadata (Content-Type/Content-Encoding) for UAS processing.
/// RFC 5621 §5: body parts with <c>handling=optional</c> MUST NOT cause rejection.
/// </summary>
internal static class SipContentPolicy
{
    /// <summary>
    /// Supported body content type for SDP offer/answer processing.
    /// </summary>
    public const string SupportedSdpContentType = "application/sdp";

    /// <summary>
    /// Supported content-coding token list.
    /// </summary>
    public const string SupportedContentEncodingList = "identity";

    /// <summary>
    /// Returns true when request body metadata is supported for SDP-capable methods.
    ///
    /// Per RFC 5621 §5: if the Content-Disposition header carries <c>handling=optional</c>
    /// the UA MUST NOT reject the request solely due to an unsupported body type.
    /// The default handling when the parameter is absent is <c>required</c>.
    /// </summary>
    public static bool TryValidateSdpRequest(
        SipRequest request,
        out int rejectionStatusCode,
        out string rejectionReasonPhrase,
        out IReadOnlyDictionary<string, string>? rejectionHeaders)
    {
        rejectionStatusCode = 0;
        rejectionReasonPhrase = string.Empty;
        rejectionHeaders = null;

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (!IsContentEncodingSupported(request.Header("Content-Encoding")))
        {
            rejectionStatusCode = 415;
            rejectionReasonPhrase = "Unsupported Media Type";
            rejectionHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Accept-Encoding"] = SupportedContentEncodingList
            };
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Body))
            return true;

        var contentType = request.Header("Content-Type");
        if (string.IsNullOrWhiteSpace(contentType)
            || !contentType.Contains(SupportedSdpContentType, StringComparison.OrdinalIgnoreCase))
        {
            // RFC 5621 §5: body with handling=optional MUST NOT cause rejection.
            if (IsHandlingOptional(request.Header("Content-Disposition")))
                return true;

            rejectionStatusCode = 415;
            rejectionReasonPhrase = "Unsupported Media Type";
            rejectionHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Accept"] = SupportedSdpContentType
            };
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true when all content-coding tokens are supported.
    /// </summary>
    private static bool IsContentEncodingSupported(string? contentEncodingHeader)
    {
        if (string.IsNullOrWhiteSpace(contentEncodingHeader))
            return true;

        var encodings = contentEncodingHeader
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var encoding in encodings)
        {
            if (!encoding.Equals("identity", StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true when the Content-Disposition header explicitly carries <c>handling=optional</c>.
    /// Per RFC 5621 §5 the default value of the handling parameter is <c>required</c>,
    /// so absence of the parameter or of the header itself is treated as required.
    /// </summary>
    internal static bool IsHandlingOptional(string? contentDisposition)
    {
        if (string.IsNullOrWhiteSpace(contentDisposition))
            return false;

        var parameters = contentDisposition
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        // Skip the disposition-type token at index 0, examine parameters.
        for (var i = 1; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var eqIdx = param.IndexOf('=');
            if (eqIdx > 0
                && param[..eqIdx].Trim().Equals("handling", StringComparison.OrdinalIgnoreCase))
            {
                var value = param[(eqIdx + 1)..].Trim().Trim('"');
                return value.Equals("optional", StringComparison.OrdinalIgnoreCase);
            }
        }

        return false;  // handling parameter absent → required (RFC 5621 §5 default)
    }
}
