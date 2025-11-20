namespace Spotless.Application.Configurations
{
    public class WhatsAppSettings
    {
        public const string SettingsKey = "WhatsAppSettings";

        public string Provider { get; set; } = "Dummy";
        public string ApiKey { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
    }
}
