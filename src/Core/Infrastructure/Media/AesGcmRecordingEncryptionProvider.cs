using System.Security.Cryptography;
using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

/// <summary>
/// AES-256-GCM reference implementation for recording file encryption.
/// </summary>
public sealed class AesGcmRecordingEncryptionProvider : IRecordingEncryptionProvider
{
    private const string Magic = "VREC1";
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private readonly byte[] _key;

    /// <summary>
    /// Creates an encryption provider from a raw 32-byte AES key.
    /// </summary>
    public AesGcmRecordingEncryptionProvider(ReadOnlySpan<byte> key)
    {
        if (key.Length != 32)
            throw new ArgumentException("AES-256-GCM key must be exactly 32 bytes.", nameof(key));

        _key = key.ToArray();
    }

    /// <summary>
    /// Creates an encryption provider by deriving a 32-byte key from passphrase+salt (PBKDF2-SHA256).
    /// </summary>
    public static AesGcmRecordingEncryptionProvider FromPassphrase(
        string passphrase,
        ReadOnlySpan<byte> salt,
        int iterations = 100_000)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase);
        if (salt.Length < 8)
            throw new ArgumentException("Salt must be at least 8 bytes.", nameof(salt));
        if (iterations < 10_000)
            throw new ArgumentOutOfRangeException(nameof(iterations), "PBKDF2 iterations must be >= 10,000.");

        var key = Rfc2898DeriveBytes.Pbkdf2(
            passphrase,
            salt.ToArray(),
            iterations,
            HashAlgorithmName.SHA256,
            32);
        return new AesGcmRecordingEncryptionProvider(key);
    }

    /// <inheritdoc />
    public string OutputFileExtension => "enc";

    /// <inheritdoc />
    public async ValueTask EncryptFileAsync(
        string inputFilePath,
        string encryptedOutputPath,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedOutputPath);

        var inputBytes = await File.ReadAllBytesAsync(inputFilePath, ct).ConfigureAwait(false);
        var ciphertext = new byte[inputBytes.Length];
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var tag = new byte[TagSize];

        using (var aes = new AesGcm(_key, TagSize))
        {
            aes.Encrypt(
                nonce,
                inputBytes,
                ciphertext,
                tag,
                associatedData: null);
        }

        var directory = Path.GetDirectoryName(encryptedOutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var stream = new FileStream(
            encryptedOutputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 8192,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);

        var header = System.Text.Encoding.ASCII.GetBytes(Magic);
        await stream.WriteAsync(header, ct).ConfigureAwait(false);
        await stream.WriteAsync(nonce, ct).ConfigureAwait(false);
        await stream.WriteAsync(tag, ct).ConfigureAwait(false);
        await stream.WriteAsync(ciphertext, ct).ConfigureAwait(false);
        await stream.FlushAsync(ct).ConfigureAwait(false);
    }
}
