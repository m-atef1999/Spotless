namespace Spotless.Application.Configurations
{
    public class RabbitMqSettings
    {
        public const string SettingsKey = "RabbitMQ";

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public bool Enabled { get; set; } = true;

        public string NotificationQueueName { get; set; } = "spotless.notifications";
        public string BillingQueueName { get; set; } = "spotless.billing";
        public string TelemetryQueueName { get; set; } = "spotless.telemetry";
        public string AnalyticsQueueName { get; set; } = "spotless.analytics";
    }
}
