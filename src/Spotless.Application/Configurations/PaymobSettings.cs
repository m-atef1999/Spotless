using System;

namespace Spotless.Application.Configurations
{
    public class PaymobSettings
    {
        public const string SettingsKey = "Paymob";
        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string HmacSecret { get; set; } = "";
        public string PublicKey { get; set; } = "";
        public int IntegrationId { get; set; }
        public int IframeId { get; set; }
    }
}
