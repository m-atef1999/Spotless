namespace Spotless.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        string EncryptToBase64(byte[] data);
        byte[] DecryptFromBase64(string base64CipherText);
    }
}

