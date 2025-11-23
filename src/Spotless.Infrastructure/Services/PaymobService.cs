using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Services
{
    public class PaymobService : IPaymentGatewayService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;

        public PaymobService(HttpClient httpClient, IOptions<PaymobSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");

            // Validate Settings
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new InvalidOperationException("Paymob ApiKey is missing. Please check your configuration (appsettings.json or Environment Variables).");
            }
            if (_settings.IntegrationId <= 0)
            {
                throw new InvalidOperationException("Paymob IntegrationId is missing or invalid. Please check your configuration.");
            }
            if (_settings.IframeId <= 0)
            {
                throw new InvalidOperationException("Paymob IframeId is missing or invalid. Please check your configuration.");
            }
        }

        public async Task<string> InitiatePaymentAsync(Guid? orderId, Money amount, string customerEmail, CancellationToken cancellationToken)
        {
            // 1. Authentication Request
            var authResponse = await _httpClient.PostAsJsonAsync("auth/tokens", new { api_key = _settings.ApiKey }, cancellationToken);
            authResponse.EnsureSuccessStatusCode();
            var authData = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponse>(cancellationToken: cancellationToken);
            var token = authData?.Token ?? throw new Exception("Failed to get Paymob auth token");

            // 2. Order Registration API
            var orderRequest = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (int)(amount.Amount * 100),
                currency = "EGP",
                merchant_order_id = orderId?.ToString() ?? $"WALLET-{Guid.NewGuid()}", // Unique ID
                items = new object[] { } // Items are optional for wallet top-up, but good to have for orders
            };

            var orderResponse = await _httpClient.PostAsJsonAsync("ecommerce/orders", orderRequest, cancellationToken);
            orderResponse.EnsureSuccessStatusCode();
            var orderData = await orderResponse.Content.ReadFromJsonAsync<PaymobOrderResponse>(cancellationToken: cancellationToken);
            var paymobOrderId = orderData?.Id ?? throw new Exception("Failed to register Paymob order");

            // 3. Payment Key Request
            var billingData = new
            {
                apartment = "NA",
                email = customerEmail,
                floor = "NA",
                first_name = "Customer", // You might want to pass real name
                street = "NA",
                building = "NA",
                phone_number = "+201000000000", // You might want to pass real phone
                shipping_method = "NA",
                postal_code = "NA",
                city = "NA",
                country = "NA",
                last_name = "User",
                state = "NA"
            };

            var keyRequest = new
            {
                auth_token = token,
                amount_cents = (int)(amount.Amount * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = billingData,
                currency = "EGP",
                integration_id = _settings.IntegrationId
            };

            var keyResponse = await _httpClient.PostAsJsonAsync("acceptance/payment_keys", keyRequest, cancellationToken);
            keyResponse.EnsureSuccessStatusCode();
            var keyData = await keyResponse.Content.ReadFromJsonAsync<PaymobKeyResponse>(cancellationToken: cancellationToken);
            var paymentKey = keyData?.Token ?? throw new Exception("Failed to get Paymob payment key");

            // 4. Construct Iframe URL
            return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentKey}";
        }

        public Task<PaymentStatus> VerifyPaymentAsync(string transactionReference, CancellationToken cancellationToken)
        {
            // Verification is usually done via Webhook (HMAC), but if we need to query:
            // We would need to use the Transaction Inquiry API.
            // For now, we'll assume the webhook handles the status update.
            // If this method is called, we might return Pending or query the API.
            
            // NOTE: The current architecture relies on Webhooks for final status.
            // This method might be used for manual checks.
            
            return Task.FromResult(PaymentStatus.Pending); 
        }

        // DTOs
        private class PaymobAuthResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; } = "";
        }

        private class PaymobOrderResponse
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
        }

        private class PaymobKeyResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; } = "";
        }
    }
}
