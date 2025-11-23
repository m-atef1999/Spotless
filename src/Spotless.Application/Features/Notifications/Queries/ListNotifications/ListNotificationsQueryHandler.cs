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
            var skip = (request.Page - 1) * request.PageSize;
            var take = request.PageSize;

            var notifications = await _unitOfWork.Notifications.GetPagedAsync(
                filter: n => n.UserId == request.UserId && (!request.UnreadOnly.HasValue || !request.UnreadOnly.Value || !n.IsRead),
                skip: skip,
                take: take,
                orderBy: q => q.OrderByDescending(n => n.CreatedAt)
            );

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }
}
