using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Secure AES-256-GCM encryption service (Recommended standard in 2025+)
    /// Provides authenticated encryption with random nonce for every operation
    /// </summary>
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public AesEncryptionService(IOptions<EncryptionSettings> settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Value.EncryptionKey))
                throw new InvalidOperationException("EncryptionSettings.EncryptionKey is missing or empty in configuration.");

            // Derive a proper 32-byte (256-bit) key using SHA-256
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(settings.Value.EncryptionKey));
        }

        // =========================== String Encryption ===========================

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var (cipherText, nonce, tag) = EncryptCore(plainBytes);

            // Format: [nonce:12][tag:16][ciphertext:...]
            var result = new byte[nonce.Length + tag.Length + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public string? Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                var data = Convert.FromBase64String(cipherText);
                if (data.Length < 28) // 12 nonce + 16 tag minimum
                    throw new CryptographicException("Invalid ciphertext length.");

                var nonce = data.AsSpan(0, 12).ToArray();
                var tag = data.AsSpan(12, 16).ToArray();
                var actualCipherText = data.AsSpan(28).ToArray();

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
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public byte[] DecryptFromBase64(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText)) return Array.Empty<byte>();

            try
            {
                var data = Convert.FromBase64String(base64CipherText);
                if (data.Length < 28)
                    throw new CryptographicException("Invalid ciphertext length.");

                var nonce = data.AsSpan(0, 12).ToArray();
                var tag = data.AsSpan(12, 16).ToArray();
                var cipherText = data.AsSpan(28).ToArray();

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
            using var aesGcm = new AesGcm(_key);

            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes recommended
            RandomNumberGenerator.Fill(nonce);

            var cipherText = new byte[plainBytes.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

            aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);

            return (cipherText, nonce, tag);
        }

        private byte[] DecryptCore(byte[] cipherText, byte[] nonce, byte[] tag)
        {
            using var aesGcm = new AesGcm(_key);

            var plainBytes = new byte[cipherText.Length];
            aesGcm.Decrypt(nonce, cipherText, tag, plainBytes);

            return plainBytes;
        }
    }
}