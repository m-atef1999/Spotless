using MediatR;
using Spotless.Application.Dtos.Notification;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Notifications.Queries.ListNotifications
{
    public class ListNotificationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListNotificationsQuery, IReadOnlyList<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<NotificationDto>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = request.UnreadOnly == true
                ? await _unitOfWork.Notifications.GetAsync(n => n.UserId == request.UserId && !n.IsRead)
                : await _unitOfWork.Notifications.GetAsync(n => n.UserId == request.UserId);

            return notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }
    }
}
