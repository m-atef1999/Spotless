using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DebugController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("config")]
        [AllowAnonymous]
        public IActionResult GetConfig()
        {
            var geminiKey = _configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            var paymobKey = _configuration["Paymob:ApiKey"] ?? Environment.GetEnvironmentVariable("Paymob__ApiKey");
            var dbConnection = _configuration.GetConnectionString("DefaultConnection");

            return Ok(new
            {
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                GeminiKeyPresent = !string.IsNullOrEmpty(geminiKey),
                GeminiKeyLength = geminiKey?.Length ?? 0,
                GeminiKeyPrefix = geminiKey?.Length > 4 ? geminiKey.Substring(0, 4) : "N/A",
                PaymobKeyPresent = !string.IsNullOrEmpty(paymobKey),
                DbConnectionPresent = !string.IsNullOrEmpty(dbConnection),
                DbConnectionServer = dbConnection?.Split(';').FirstOrDefault(x => x.Contains("Server")) ?? "Unknown"
            });
        }
    }
}
