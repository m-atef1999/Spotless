namespace Spotless.Infrastructure.Configurations
{
    public class ExternalServicesSettings
    {
        public const string SettingsKey = "ExternalServices";

        public string PaymentGatewayApiKey { get; set; } = string.Empty;
        public string PaymentGatewayWebhookSecret { get; set; } = string.Empty;
    }
}
