namespace Spotless.Application.Interfaces
{
    public interface ISmsService
    {

        Task<bool> SendOtpAsync(string phoneNumber);

        Task<bool> VerifyOtpAsync(string phoneNumber, string code);
    }
}
