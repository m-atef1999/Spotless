using MediatR;

namespace Spotless.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<Unit>;
}
