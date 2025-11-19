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

        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;
    }
}
