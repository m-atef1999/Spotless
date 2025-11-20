using Spotless.Application.Interfaces;
using System.Diagnostics;

namespace Spotless.Infrastructure.Services
{
    // Implementation for development: logs the code instead of sending SMS.
    public class DummySmsService : ISmsService
    {

        private const string MockOtpCode = "123456";

        public Task<bool> SendOtpAsync(string phoneNumber)
        {
            Debug.WriteLine($"[SMS DEBUG] OTP request received for: {phoneNumber}");
            Debug.WriteLine($"[SMS DEBUG] **MOCK OTP CODE: {MockOtpCode}** (Use this code for verification)");

            return Task.FromResult(true);
        }

        public Task<bool> VerifyOtpAsync(string phoneNumber, string code)
        {

            bool isValid = code == MockOtpCode;

            if (isValid)
            {
                Debug.WriteLine($"[SMS DEBUG] Verification SUCCESS for {phoneNumber}.");
            }
            else
            {
                Debug.WriteLine($"[SMS DEBUG] Verification FAILED for {phoneNumber}. Code provided: {code}.");
            }

            return Task.FromResult(isValid);
        }
    }
}
