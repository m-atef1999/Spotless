using MediatR;
using Spotless.Application.Dtos.Notification;

namespace Spotless.Application.Features.Notifications.Queries.ListNotifications
{
    public record ListNotificationsQuery(Guid UserId, bool? UnreadOnly = null) : IRequest<IReadOnlyList<NotificationDto>>;
}
