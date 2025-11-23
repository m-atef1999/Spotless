using Microsoft.AspNetCore.SignalR;
using Spotless.API.Hubs;
using Spotless.Application.Interfaces;

namespace Spotless.API.Services
{
    public class SignalRPushNotificationSender(IHubContext<NotificationHub> hubContext) : IPushNotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public async Task SendNotificationAsync(string userId, string title, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new 
            { 
                Title = title, 
                Message = message, 
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
