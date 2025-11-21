using Microsoft.Extensions.Configuration;
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
        private readonly HttpClient _httpClient;

        public AiService(IConfiguration configuration, IServiceRepository serviceRepository)
        {
            _configuration = configuration;
            _serviceRepository = serviceRepository;
            _httpClient = new HttpClient();
        }

        public async Task<ChatResponse> GetResponseAsync(string userMessage)
        {
            var apiKey = _configuration["OPENAI_API_KEY"];
            
            // Hybrid approach: Try OpenAI if key exists, otherwise fallback to local logic
            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    return await GetOpenAiResponseAsync(userMessage, apiKey);
                }
                catch
                {
                    // Fallback if API fails
                    return await GetLocalResponseAsync(userMessage);
                }
            }

            return await GetLocalResponseAsync(userMessage);
        }

        private async Task<ChatResponse> GetOpenAiResponseAsync(string message, string apiKey)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are Spotless AI, a helpful assistant for a laundry and dry cleaning service. Be concise and friendly." },
                    new { role = "user", content = message }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var reply = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return new ChatResponse { Response = reply };
            }

            throw new Exception("OpenAI API failed");
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
                    sb.AppendLine($"- {s.Name}: ${s.BasePrice}");
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
