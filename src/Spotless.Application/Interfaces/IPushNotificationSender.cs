namespace Spotless.Application.Interfaces
{
    public interface IPushNotificationSender
    {
        Task SendNotificationAsync(string userId, string title, string message);
    }
}
