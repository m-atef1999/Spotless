using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spotless.Application.Dtos.Ai;
using Spotless.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace Spotless.API.Services
{
    public interface IAiService
    {
        Task<ChatResponse> GetResponseAsync(string userMessage);
    }

    public class AiService : IAiService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IRepository<Domain.Entities.Customer> _customerRepository;
        private readonly IRepository<Domain.Entities.Driver> _driverRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiService> _logger;

        public AiService(
            IConfiguration configuration, 
            IServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IRepository<Domain.Entities.Customer> customerRepository,
            IRepository<Domain.Entities.Driver> driverRepository,
            ILogger<AiService> logger)
        {
            _configuration = configuration;
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _driverRepository = driverRepository;
            _httpClient = new HttpClient();
            _logger = logger;
            Console.WriteLine("[AiService] Service initialized (Gemini Edition).");
        }

        public async Task<ChatResponse> GetResponseAsync(string userMessage)
        {
            _logger.LogWarning($"[AiService] Processing message: {userMessage}");
            
            
            // Prioritize Environment Variable for Security
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            
            // Fallback to appsettings.json
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = _configuration["Gemini:ApiKey"];
            }

            
            
            
            // Hybrid approach: Try Gemini if key exists, otherwise fallback to local logic
            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    _logger.LogWarning($"[AiService] Using Gemini API Key: {apiKey.Substring(0, 3)}...{apiKey.Substring(apiKey.Length - 3)}");
                    return await GetGeminiResponseAsync(userMessage, apiKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"[AiService] Gemini API Error: {ex.Message}");
                    _logger.LogError(ex, "Gemini API failed. Falling back to local logic.");
                    
                    
                    // Fallback if API fails
                    return await GetLocalResponseAsync(userMessage);
                }
            }
            else
            {
                _logger.LogWarning("[AiService] GEMINI_API_KEY is missing or empty.");
                _logger.LogWarning("GEMINI_API_KEY is missing or empty. Using local logic.");
            }

            return await GetLocalResponseAsync(userMessage);
        }

        private async Task<ChatResponse> GetGeminiResponseAsync(string message, string apiKey)
        {
            var systemPrompt = new StringBuilder("You are Spotless AI, a helpful assistant for a laundry and dry cleaning service. Be concise and friendly.");
            
            if (_currentUserService.IsAuthenticated && !string.IsNullOrEmpty(_currentUserService.UserId))
            {
                var userSummary = await GetUserSummaryAsync(_currentUserService.UserId);
                systemPrompt.Append($"\n\nUser Context:\n{userSummary}");
                systemPrompt.Append("\n\nUse this context to answer questions about the user's orders, cart, wallet, or account. If they ask about something else, answer generally.");
                systemPrompt.Append("\n\nIMPORTANT: When referring to an order, prefer using its STATUS (e.g., 'your Requested order') or Service Name instead of the Order ID, unless the user explicitly asks for the ID.");
                systemPrompt.Append("\n\nIMPORTANT: The system uses an In-Memory Database for development. Order IDs and history may reset if the server restarts. Explain this if the user asks why their orders changed.");
                _logger.LogWarning($"[AiService] System Prompt: {systemPrompt}");
            }

            // Gemini API Payload Structure
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt.ToString() + "\n\nUser: " + message }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}";
            
            var response = await _httpClient.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                
                // Parse Gemini Response: candidates[0].content.parts[0].text
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var contentElem) && 
                        contentElem.TryGetProperty("parts", out var parts) && 
                        parts.GetArrayLength() > 0)
                    {
                        var reply = parts[0].GetProperty("text").GetString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(reply))
                        {
                            reply = "I'm sorry — I couldn't get a proper response right now. Try again later.";
                        }

                        return new ChatResponse { Response = reply };
                    }
                }
                
                throw new Exception("Gemini API response format unexpected.");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API failed: {response.StatusCode} - {errorContent}");
        }

        private async Task<string> GetUserSummaryAsync(string userId)
        {
            var sb = new StringBuilder();
            
            // 1. Get Name (from claims)
            var name = _currentUserService.User?.Identity?.Name ?? "Valued Customer";
            sb.AppendLine($"Name: {name}");

            var customerId = _currentUserService.CustomerId;

            if (customerId.HasValue)
            {
                // 1.1 Get Customer Details (Wallet, Address)
                try
                {
                    var customer = await _customerRepository.GetByIdAsync(customerId.Value);
                    if (customer != null)
                    {
                        sb.AppendLine($"Wallet Balance: {customer.WalletBalance?.Amount ?? 0} {customer.WalletBalance?.Currency ?? "USD"}");
                        if (customer.Address != null)
                        {
                            sb.AppendLine($"Address: {customer.Address.Street}, {customer.Address.City}");
                        }
                    }
                }
                catch (Exception ex)
                { 
                    _logger.LogError(ex, "[AiService] Error retrieving customer details.");
                    sb.AppendLine("Could not retrieve wallet details."); 
                }

                // 2. Get Active Orders
                try 
                {
                    var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId.Value);
                    var activeOrders = orders
                        .OrderByDescending(o => o.OrderDate)
                        .Take(10) // Limit to 10 most recent for context window
                        .ToList();
                    
                    if (activeOrders.Any())
                    {
                        sb.AppendLine($"Recent Orders ({activeOrders.Count}):");
                        foreach(var order in activeOrders)
                        {
                            var serviceNames = order.Items.Any() ? string.Join(", ", order.Items.Select(i => i.Service?.Name ?? "Service")) : "Service";
                            sb.AppendLine($"- Order #{order.Id.ToString().Substring(0, 8)} ({order.OrderDate:MMM dd}): {order.Status} - {serviceNames} (Total: ${order.TotalPrice.Amount})");
                        }
                    }
                    else
                    {
                        sb.AppendLine("No recent orders.");
                    }
                }
                catch { sb.AppendLine("Could not retrieve orders."); }

                // 3. Get Cart Info
                try
                {
                    var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId.Value);
                    if (cart != null && cart.Items.Any())
                    {
                        sb.AppendLine($"Cart: {cart.Items.Count} items");
                    }
                    else
                    {
                        sb.AppendLine("Cart is empty.");
                    }
                }
                catch { sb.AppendLine("Could not retrieve cart."); }
            }
            else
            {
                sb.AppendLine("User is not a registered customer (no CustomerId found).");
            }

            // 4. Check Driver Status (if applicable)
            // Assuming userId matches Driver ID or we can find driver by email/identity
            // For now, we'll skip complex driver lookup unless requested, as usually drivers have a separate role.
            if (_currentUserService.User?.IsInRole("Driver") == true)
            {
                 sb.AppendLine("User is a Driver.");
            }

            return sb.ToString();
        }

        private async Task<ChatResponse> GetLocalResponseAsync(string message)
        {
            var lowerMessage = message.ToLower();
            var response = "I'm Spotless AI. How can I help you today?";

            if (lowerMessage.Contains("price") || lowerMessage.Contains("cost") || lowerMessage.Contains("how much"))
            {
                var services = await _serviceRepository.GetAllAsync();
                var sb = new StringBuilder("Here are some of our prices:\n");
                foreach (var s in services.Take(5))
                {
                    sb.AppendLine($"- {s.Name}: ${s.BasePrice.Amount}");
                }
                response = sb.ToString();
            }
            else if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
            {
                response = "Hello! Welcome to Spotless. I can help you check prices or services.";
            }
            else if (lowerMessage.Contains("service") || lowerMessage.Contains("cleaning"))
            {
                response = "We offer Dry Cleaning, Laundry, Ironing, and more. Check our Services page for details!";
            }
            else if (lowerMessage.Contains("time") || lowerMessage.Contains("hours"))
            {
                response = "We are open 24/7 for online orders. Pickup and delivery times are available from 8 AM to 8 PM.";
            }
            else
            {
                response = "I'm not sure about that, but our support team is always happy to help! You can also check our Services page for more info.";
            }

            return new ChatResponse { Response = response };
        }
    }
}
