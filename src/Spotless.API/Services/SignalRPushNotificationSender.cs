using Microsoft.AspNetCore.SignalR;
using Spotless.API.Hubs;
using Spotless.Application.Interfaces;

using Microsoft.Extensions.Logging;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.API.Services
{
    public class SignalRPushNotificationSender(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, ILogger<SignalRPushNotificationSender> logger) : IPushNotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SignalRPushNotificationSender> _logger = logger;

        public async Task SendNotificationAsync(string userId, string title, string message)
        {
            // 1. Persist to DB
            if (Guid.TryParse(userId, out var userGuid))
            {
                try
                {
                    var notification = new Notification(userGuid, title, message, NotificationType.System);
                    await _unitOfWork.Notifications.AddAsync(notification);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist notification for user {UserId}", userId);
                }
            }
            else
            {
                _logger.LogWarning("Invalid UserId format for notification: {UserId}", userId);
            }

            // 2. Send Real-time
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new 
            { 
                Title = title, 
                Message = message, 
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
