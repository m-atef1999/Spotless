using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Spotless.Infrastructure.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly EncryptionSettings _settings;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionService(IOptions<EncryptionSettings> settings)
        {
            _settings = settings.Value;

            if (string.IsNullOrEmpty(_settings.EncryptionKey))
            {
                throw new InvalidOperationException("EncryptionSettings.EncryptionKey is not configured.");
            }


            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(_settings.EncryptionKey));


            using var md5 = MD5.Create();
            _iv = md5.ComputeHash(_key)[..16];
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var msDecrypt = new MemoryStream(cipherBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
            catch
            {
                return cipherText;
            }
        }

        public string EncryptToBase64(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(data, 0, data.Length);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public byte[] DecryptFromBase64(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText))
                return Array.Empty<byte>();

            try
            {
                var cipherBytes = Convert.FromBase64String(base64CipherText);

                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var msDecrypt = new MemoryStream(cipherBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var msOutput = new MemoryStream();

                csDecrypt.CopyTo(msOutput);
                return msOutput.ToArray();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }
    }
}

