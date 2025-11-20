namespace Spotless.Application.Configurations
{
    public class ExternalServicesSettings
    {
        public const string SettingsKey = "ExternalServices";

        public string PaymentGatewayApiKey { get; set; } = string.Empty;
        public string PaymentGatewayWebhookSecret { get; set; } = string.Empty;
    }

    public class PaymobSettings
    {
        public const string SettingsKey = "Paymob";
        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string HmacSecret { get; set; } = "";
        public string PublicKey { get; set; } = "";
    }
}
