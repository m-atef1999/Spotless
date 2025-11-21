using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Notification() { }

        public Notification(Guid userId, string title, string message, NotificationType type)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
