using System;

namespace Spotless.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string EventType { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
