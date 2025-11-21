using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Notification
{
    public record NotificationDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public NotificationType Type { get; init; }
        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
