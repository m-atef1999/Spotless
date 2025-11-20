using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Features.Authentication.Commands.ChangePassword;
using Spotless.Application.Features.Authentication.Commands.ConfirmEmail;
using Spotless.Application.Features.Authentication.Commands.ForgotPassword;
using Spotless.Application.Features.Authentication.Commands.LoginCommand;
using Spotless.Application.Features.Authentication.Commands.RefreshToken;
using Spotless.Application.Features.Authentication.Commands.ResetPassword;
using Spotless.Application.Features.Authentication.Commands.SendOtp;
using Spotless.Application.Features.Authentication.Commands.SendVerificationEmail;
using Spotless.Application.Features.Authentication.Commands.VerifyOtp;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    using Spotless.API.Attributes;

    [ApiController]
    [Route("api/[controller]")]
    [Audit]
    public class AuthController(IMediator mediator, Microsoft.AspNetCore.Identity.UserManager<Spotless.Infrastructure.Identity.ApplicationUser> userManager, Spotless.Application.Interfaces.IAuthService authService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Spotless.Infrastructure.Identity.ApplicationUser> _userManager = userManager;
        private readonly Spotless.Application.Interfaces.IAuthService _authService = authService;

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(command with { UserId = Guid.Parse(userId) });

            if (!result)
            {
                return BadRequest(new { Message = "Password change failed. Check your current password." });
            }

            return Ok("Password successfully updated.");
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok("If the email exists in our system, a password reset link has been sent.");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = "Password reset failed. The user ID or token may be invalid or expired." });
            }

            return Ok("Password successfully reset.");
        }

        [Authorize]
        [HttpPost("verify-email/send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationEmailCommand command)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Authorization token is invalid or missing User ID claim." });
            }

            var commandWithId = command with { UserId = userId };

            var success = await _mediator.Send(commandWithId);

            if (!success)
            {
                return BadRequest(new { Message = "Could not process request for this user." });
            }

            return Ok("Verification email sent to your registered address.");
        }

        [HttpGet("verify-email/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = "Email confirmation failed. The link may be invalid or expired." });
            }

            return Ok("Email successfully confirmed. You can now log in.");
        }

        [HttpPost("verify-phone/send-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendPhoneVerification([FromBody] SendOtpCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = "Service failed to initiate OTP sending. Please try again." });
            }

            return Ok("Verification code sent successfully (or request processed).");
        }

        [HttpPost("verify-phone/confirm-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmPhoneVerification([FromBody] VerifyOtpCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = "Verification failed. Invalid code, invalid phone number, or user not found." });
            }

            return Ok("Phone number successfully verified.");
        }

        [HttpPost("external/google")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExternalGoogleLogin([FromBody] Spotless.Application.Dtos.Authentication.ExternalAuthRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Provider) || string.IsNullOrEmpty(request.IdToken))
                return BadRequest(new { Message = "Provider and IdToken are required." });

            try
            {
                var result = await _authService.ExternalLoginAsync(request.Provider, request.IdToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
