using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    public class PaymobSignatureService : IPaymobSignatureService
    {
        private readonly PaymobSettings _paymobSettings;

        public PaymobSignatureService(IOptions<PaymobSettings> paymobSettings)
        {
            _paymobSettings = paymobSettings.Value;
        }


        public bool VerifyProcessedCallbackSignature(Spotless.Application.Interfaces.PaymobProcessedCallbackData callbackData, string receivedHmac)
        {
            if (string.IsNullOrEmpty(receivedHmac))
                return false;

            var concatenatedString = $"{callbackData.AmountCents}" +
                                     $"{callbackData.CreatedAt}" +
                                     $"{callbackData.Currency}" +
                                     $"{callbackData.ErrorOccured.ToString().ToLower()}" +
                                     $"{callbackData.HasParentTransaction.ToString().ToLower()}" +
                                     $"{callbackData.Id}" +
                                     $"{callbackData.IntegrationId}" +
                                     $"{callbackData.Is3dSecure.ToString().ToLower()}" +
                                     $"{callbackData.IsAuth.ToString().ToLower()}" +
                                     $"{callbackData.IsCapture.ToString().ToLower()}" +
                                     $"{callbackData.IsRefunded.ToString().ToLower()}" +
                                     $"{callbackData.IsStandalonePayment.ToString().ToLower()}" +
                                     $"{callbackData.IsVoided.ToString().ToLower()}" +
                                     $"{callbackData.OrderId}" +
                                     $"{callbackData.Owner}" +
                                     $"{callbackData.Pending.ToString().ToLower()}" +
                                     $"{callbackData.SourceDataPan}" +
                                     $"{callbackData.SourceDataSubType}" +
                                     $"{callbackData.SourceDataType}" +
                                     $"{callbackData.Success.ToString().ToLower()}";

            // Calculate HMAC-SHA512
            var calculatedHmac = CalculateHmacSha512(concatenatedString, _paymobSettings.HmacSecret);

            // Compare with received HMAC
            return string.Equals(calculatedHmac, receivedHmac, StringComparison.OrdinalIgnoreCase);
        }

 
        public bool VerifyResponseCallbackSignature(PaymobResponseCallbackData callbackData, string receivedHmac)
        {
            if (string.IsNullOrEmpty(receivedHmac))
                return false;


            var concatenatedString = $"{callbackData.AmountCents}" +
                                     $"{callbackData.CreatedAt}" +
                                     $"{callbackData.Currency}" +
                                     $"{callbackData.ErrorOccured.ToString().ToLower()}" +
                                     $"{callbackData.HasParentTransaction.ToString().ToLower()}" +
                                     $"{callbackData.Id}" +
                                     $"{callbackData.IntegrationId}" +
                                     $"{callbackData.Is3dSecure.ToString().ToLower()}" +
                                     $"{callbackData.IsAuth.ToString().ToLower()}" +
                                     $"{callbackData.IsCapture.ToString().ToLower()}" +
                                     $"{callbackData.IsRefunded.ToString().ToLower()}" +
                                     $"{callbackData.IsStandalonePayment.ToString().ToLower()}" +
                                     $"{callbackData.IsVoided.ToString().ToLower()}" +
                                     $"{callbackData.OrderId}" +
                                     $"{callbackData.Owner}" +
                                     $"{callbackData.Pending.ToString().ToLower()}" +
                                     $"{callbackData.SourceDataPan}" +
                                     $"{callbackData.SourceDataSubType}" +
                                     $"{callbackData.SourceDataType}" +
                                     $"{callbackData.Success.ToString().ToLower()}";

            //  Calculate HMAC-SHA512
            var calculatedHmac = CalculateHmacSha512(concatenatedString, _paymobSettings.HmacSecret);

            //  Compare with received HMAC
            return string.Equals(calculatedHmac, receivedHmac, StringComparison.OrdinalIgnoreCase);
        }


        private string CalculateHmacSha512(string data, string secretKey)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secretKey);
            var dataBytes = encoding.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            // Convert to hexadecimal string
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
