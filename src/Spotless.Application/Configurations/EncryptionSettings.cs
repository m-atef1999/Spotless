namespace Spotless.Application.Configurations
{
    public class EncryptionSettings
    {
        public const string SettingsKey = "EncryptionSettings";

        public string EncryptionKey { get; set; } = string.Empty;
    }
}

