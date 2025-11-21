using MediatR;

namespace Spotless.Application.Features.Notifications.Commands.DeleteNotification
{
    public record DeleteNotificationCommand(Guid NotificationId, Guid UserId) : IRequest<Unit>;
}
