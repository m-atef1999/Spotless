namespace Spotless.Infrastructure.Configurations
{
    public class PaymentGatewaySettings
    {
        public const string SettingsKey = "PaymentGateway";

        public string ApiKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
