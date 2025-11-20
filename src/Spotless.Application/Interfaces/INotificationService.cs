namespace Spotless.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailNotificationAsync(string email, string subject, string message);
        Task SendSmsNotificationAsync(string phoneNumber, string message);
        Task SendWhatsAppNotificationAsync(string phoneNumber, string message);
        Task SendPushNotificationAsync(string userId, string title, string message);
    }
}