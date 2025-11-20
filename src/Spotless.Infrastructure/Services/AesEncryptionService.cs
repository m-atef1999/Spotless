using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Secure AES-256-GCM encryption service (Recommended standard)
    /// Provides authenticated encryption with random nonce for every operation.
    /// </summary>
    public class AesEncryptionService : IEncryptionService
    {
        // Recommended sizes for GCM
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int MinimumCipherLength = NonceSize + TagSize;

        private readonly byte[] _key;

        public AesEncryptionService(IOptions<EncryptionSettings> settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Value.EncryptionKey))
                throw new InvalidOperationException("EncryptionSettings.EncryptionKey is missing or empty in configuration.");

            // Derive a proper 32-byte (256-bit) key using SHA-256
            // The using statement automatically handles disposal of the SHA256 object.
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(settings.Value.EncryptionKey));
        }

        // =========================== String Encryption ===========================

        // Note: The nullability warning (string?) is fixed by using 'string' for IEncryptionService.Decrypt
        // and ensuring we return a non-null string (or throw) where expected.

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var (cipherText, nonce, tag) = EncryptCore(plainBytes);

            // Format: [nonce:12][tag:16][ciphertext:...]
            var result = new byte[nonce.Length + tag.Length + cipherText.Length];

            // Use CopyTo for modern Span-based array copying
            nonce.CopyTo(result.AsSpan(0));
            tag.CopyTo(result.AsSpan(nonce.Length));
            cipherText.CopyTo(result.AsSpan(nonce.Length + tag.Length));

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText) // Removed '?' to fix nullability warning
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                var data = Convert.FromBase64String(cipherText);
                if (data.Length < MinimumCipherLength)
                    throw new CryptographicException("Invalid ciphertext length.");

                // Use Span<byte> for efficient slicing and allocation reduction
                var nonce = data.AsSpan(0, NonceSize);
                var tag = data.AsSpan(NonceSize, TagSize);
                var actualCipherText = data.AsSpan(MinimumCipherLength);

                var plainBytes = DecryptCore(actualCipherText, nonce, tag);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (CryptographicException ex)
            {
                // Tampered or wrong key → do NOT silently fail
                throw new CryptographicException("Failed to decrypt data. Possible data tampering or incorrect key.", ex);
            }
        }

        // =========================== Byte[] Encryption ===========================

        public string EncryptToBase64(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;

            var (cipherText, nonce, tag) = EncryptCore(data);

            var result = new byte[nonce.Length + tag.Length + cipherText.Length];
            nonce.CopyTo(result.AsSpan(0));
            tag.CopyTo(result.AsSpan(nonce.Length));
            cipherText.CopyTo(result.AsSpan(nonce.Length + tag.Length));

            return Convert.ToBase64String(result);
        }

        public byte[] DecryptFromBase64(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText)) return Array.Empty<byte>();

            try
            {
                var data = Convert.FromBase64String(base64CipherText);
                if (data.Length < MinimumCipherLength)
                    throw new CryptographicException("Invalid ciphertext length.");

                var nonce = data.AsSpan(0, NonceSize);
                var tag = data.AsSpan(NonceSize, TagSize);
                var cipherText = data.AsSpan(MinimumCipherLength);

                return DecryptCore(cipherText, nonce, tag);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Failed to decrypt data. Possible data tampering or incorrect key.", ex);
            }
        }

        // =========================== Core AES-GCM Implementation ===========================

        private (byte[] cipherText, byte[] nonce, byte[] tag) EncryptCore(byte[] plainBytes)
        {
            // FIX: Use the non-obsolete constructor that takes the tag size.
            using var aesGcm = new AesGcm(_key, TagSize);

            var nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            var cipherText = new byte[plainBytes.Length];
            var tag = new byte[TagSize];

            aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);

            return (cipherText, nonce, tag);
        }

        private byte[] DecryptCore(ReadOnlySpan<byte> cipherText, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> tag)
        {
            // FIX: Use the non-obsolete constructor that takes the tag size.
            using var aesGcm = new AesGcm(_key, TagSize);

            // Allocation of plainBytes moved here
            var plainBytes = new byte[cipherText.Length];

            // Decrypt accepts ReadOnlySpan for efficiency
            aesGcm.Decrypt(nonce, cipherText, tag, plainBytes);

            return plainBytes;
        }
    }
}
