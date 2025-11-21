using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteNotificationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId)
                ?? throw new KeyNotFoundException($"Notification with ID {request.NotificationId} not found");

            // Verify the notification belongs to the user
            if (notification.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to delete this notification");

            await _unitOfWork.Notifications.DeleteAsync(notification);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
