using Microsoft.Extensions.Options;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;
using Spotless.Infrastructure.Configurations;

namespace Spotless.Infrastructure.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly PaymentGatewaySettings _settings;
        private readonly IEncryptionService _encryptionService;
        private readonly HttpClient _httpClient;


        public PaymentGatewayService(
            IOptions<PaymentGatewaySettings> options,
            IEncryptionService encryptionService)
        {
            _settings = options.Value;
            _encryptionService = encryptionService;


            var baseUrl = _settings.BaseUrl;

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };


            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {

                var apiKey = _encryptionService.Decrypt(_settings.ApiKey);
                if (!string.IsNullOrEmpty(apiKey) && apiKey != _settings.ApiKey)
                {
                    _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
                }
            }
        }

        public Task<string> InitiatePaymentAsync(
            Guid? orderId,
            Money amount,
            string customerEmail,
            CancellationToken cancellationToken)
        {


            Console.WriteLine($"Initiating payment for Order {orderId?.ToString() ?? "Wallet TopUp"} of {amount.Amount} {amount.Currency} via {_settings.BaseUrl}");


            string mockTransactionReference = Guid.NewGuid().ToString();
            string orderParam = orderId.HasValue ? orderId.Value.ToString() : "wallet-topup";

            return Task.FromResult($"https://checkout.gateway.com/pay/{mockTransactionReference}?order={orderParam}");
        }

        public Task<PaymentStatus> VerifyPaymentAsync(string transactionReference, CancellationToken cancellationToken)
        {
            _ = _encryptionService.Decrypt(_settings.WebhookSecret);

            if (string.IsNullOrEmpty(transactionReference))
            {
                return Task.FromResult(PaymentStatus.Failed);
            }

            bool success = new Random().Next(1, 10) > 1;

            return Task.FromResult(success ? PaymentStatus.Completed : PaymentStatus.Failed);
        }
    }
}
