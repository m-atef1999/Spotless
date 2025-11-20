using System;

namespace Spotless.Application.Interfaces
{
    /// <summary>
    /// Job to send multi-channel notifications asynchronously.
    /// </summary>
    public class NotificationJob : IBackgroundJob
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CustomerId { get; set; }
        public Guid? DriverId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public bool SendWhatsApp { get; set; }

        public string EmailSubject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
        public string SmsMessage { get; set; } = string.Empty;
        public string WhatsAppMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Job to record driver telemetry for analytics and positioning.
    /// </summary>
    public class TelemetryJob : IBackgroundJob
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid DriverId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Speed { get; set; }
        public int Heading { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Job to process analytics events (order created, driver assigned, etc.).
    /// </summary>
    public class AnalyticsJob : IBackgroundJob
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string EventType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = [];
    }

    /// <summary>
    /// Job to process billing/payment events.
    /// </summary>
    public class BillingJob : IBackgroundJob
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string BillingEventType { get; set; } = string.Empty; // "PaymentCompleted", "Refund", etc.
    }
}
