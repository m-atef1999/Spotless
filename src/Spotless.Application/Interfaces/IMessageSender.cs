namespace Spotless.Application.Interfaces
{
    public interface IMessageSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendWhatsAppAsync(string phoneNumber, string message);
    }
}
