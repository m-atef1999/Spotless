using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<MarkAsReadCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId)
                ?? throw new KeyNotFoundException($"Notification with ID {request.NotificationId} not found");

            // Verify the notification belongs to the user
            if (notification.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this notification");

            notification.MarkAsRead();
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
