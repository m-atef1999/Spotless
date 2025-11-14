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

        private readonly HttpClient _httpClient;


        public PaymentGatewayService(IOptions<PaymentGatewaySettings> options)
        {
            _settings = options.Value;


            _httpClient = new HttpClient { BaseAddress = new Uri(_settings.BaseUrl) };
        }

        public Task<string> InitiatePaymentAsync(
            Guid orderId,
            Money amount,
            string customerEmail,
            CancellationToken cancellationToken)
        {


            Console.WriteLine($"Initiating payment for Order {orderId} of {amount.Amount} {amount.Currency} via {_settings.BaseUrl}");


            string mockTransactionReference = Guid.NewGuid().ToString();

            return Task.FromResult($"https://checkout.gateway.com/pay/{mockTransactionReference}?order={orderId}");
        }

        public Task<PaymentStatus> VerifyPaymentAsync(string transactionReference, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(transactionReference))
            {
                return Task.FromResult(PaymentStatus.Failed);
            }

            bool success = new Random().Next(1, 10) > 1;

            return Task.FromResult(success ? PaymentStatus.Completed : PaymentStatus.Failed);
        }
    }
}
