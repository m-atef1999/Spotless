using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Features.Payments.Commands.ProcessWebhook;
using Spotless.Application.Interfaces;
using Spotless.Domain.Exceptions;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<PaymentsController> _logger = logger;

        /// <summary>
        /// Initiates a payment for an order
        /// </summary>
        [HttpPost("initiate")]
        [Authorize]
        [ProducesResponseType(typeof(Spotless.Application.Dtos.Payment.InitiatePaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InitiatePayment(
            [FromBody] Spotless.Application.Dtos.Payment.InitiatePaymentDto dto)
        {
            try
            {
                var command = new Spotless.Application.Features.Payments.Commands.InitiatePayment.InitiatePaymentCommand(
                    dto.OrderId,
                    dto.PaymentMethod,
                    dto.ReturnUrl);

                var result = await _mediator.Send(command);

                var response = new Spotless.Application.Dtos.Payment.InitiatePaymentResponseDto
                {
                    PaymentId = result.PaymentId,
                    PaymentUrl = result.PaymentUrl,
                    TransactionReference = result.TransactionReference
                };

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Processes Paymob payment webhook notifications
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ProcessWebhook(
            [FromHeader(Name = "hmac")] string hmacSignature,
            [FromBody] PaymobProcessedCallbackData callbackData)
        {
            try
            {
                _logger.LogInformation("Received Paymob webhook with signature {Signature} for transaction {TransactionId}",
                    hmacSignature, callbackData.Id);

                if (string.IsNullOrEmpty(hmacSignature))
                {
                    _logger.LogWarning("Paymob webhook missing HMAC signature");
                    return BadRequest("Missing HMAC signature");
                }

                if (callbackData == null)
                {
                    _logger.LogWarning("Paymob webhook missing callback data");
                    return BadRequest("Missing callback data");
                }


                var command = new ProcessWebhookCommand(hmacSignature, callbackData);
                await _mediator.Send(command);

                _logger.LogInformation("Successfully processed Paymob webhook for transaction {TransactionId}",
                    callbackData.Id);

                return Ok(new { status = "processed", transactionId = callbackData.Id });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Paymob webhook signature verification failed for transaction {TransactionId}",
                    callbackData?.Id);
                return Unauthorized(new { error = "Invalid webhook signature", transactionId = callbackData?.Id });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Paymob webhook invalid data: {Message}", ex.Message);
                return BadRequest(new { error = "Invalid webhook data", message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Paymob webhook payment record not found: {Message}", ex.Message);
                return NotFound(new { error = "Payment record not found", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob webhook processing failed for transaction {TransactionId}",
                    callbackData?.Id);
                return StatusCode(500, new { error = "Internal server error", transactionId = callbackData?.Id });
            }
        }


        /// <summary>
        /// Health check endpoint for payment service
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "payments", timestamp = DateTime.UtcNow });
        }
    }
}
