using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Notification;
using Spotless.Application.Features.Notifications.Commands.DeleteNotification;
using Spotless.Application.Features.Notifications.Commands.MarkAsRead;
using Spotless.Application.Features.Notifications.Queries.ListNotifications;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        
        
        /// <summary>
        /// Retrieves notifications for the authenticated user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), 200)]
        public async Task<IActionResult> ListNotifications([FromQuery] bool? unreadOnly, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var query = new ListNotificationsQuery(userId, unreadOnly, page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Marks a notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetCurrentUserId();
            var command = new MarkAsReadCommand(id, userId);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes a notification
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var userId = GetCurrentUserId();
            var command = new DeleteNotificationCommand(id, userId);
            await _mediator.Send(command);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User identity claim is missing");

            if (!Guid.TryParse(userIdString, out Guid userId))
                throw new UnauthorizedAccessException("Invalid user identifier");

            return userId;
        }
    }
}
