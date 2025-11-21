using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.API.Services;
using Spotless.Application.Dtos.Ai;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// Chat with Spotless AI Assistant
        /// </summary>
        [HttpPost("chat")]
        [AllowAnonymous] // Allow guests to ask questions too
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            var response = await _aiService.GetResponseAsync(request.Message);
            return Ok(response);
        }
    }
}
