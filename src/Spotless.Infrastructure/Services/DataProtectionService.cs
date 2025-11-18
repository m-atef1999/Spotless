using Microsoft.AspNetCore.DataProtection;
using Spotless.Application.Interfaces;
using System.Text;

namespace Spotless.Infrastructure.Services
{

    public class DataProtectionService : IEncryptionService
    {
        private readonly IDataProtector _protector;

        public DataProtectionService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("Spotless.SensitiveData");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var protectedBytes = _protector.Protect(Encoding.UTF8.GetBytes(plainText));
            return Convert.ToBase64String(protectedBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var protectedBytes = Convert.FromBase64String(cipherText);
                var unprotectedBytes = _protector.Unprotect(protectedBytes);
                return Encoding.UTF8.GetString(unprotectedBytes);
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

            var protectedBytes = _protector.Protect(data);
            return Convert.ToBase64String(protectedBytes);
        }

        public byte[] DecryptFromBase64(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText))
                return Array.Empty<byte>();

            try
            {
                var protectedBytes = Convert.FromBase64String(base64CipherText);
                return _protector.Unprotect(protectedBytes);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }
    }
}

