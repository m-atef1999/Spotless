namespace Spotless.Application.Dtos.AuditLog
{
    public record AuditLogDto
    {
        public Guid Id { get; init; }
        public string EventType { get; init; } = string.Empty;
        public Guid? UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Data { get; init; } = string.Empty;
        public string IpAddress { get; init; } = string.Empty;
        public string CorrelationId { get; init; } = string.Empty;
        public DateTime OccurredAt { get; init; }
    }
}
