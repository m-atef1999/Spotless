using MediatR;
using Spotless.Application.Dtos.Notification;

namespace Spotless.Application.Features.Notifications.Queries.ListNotifications
{
    public record ListNotificationsQuery(Guid UserId, bool? UnreadOnly, int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<NotificationDto>>;
}
