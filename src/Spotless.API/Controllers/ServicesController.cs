using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Features.Services.Commands.CreateService;
using Spotless.Application.Features.Services.Commands.UpdateService;
using Spotless.Application.Features.Services.Commands.DeleteService;
using Spotless.Application.Features.Services.Queries.GetFeaturedServices;
using Spotless.Application.Features.Services.Queries.ListAllServices;
using Spotless.Application.Interfaces;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController(IMediator mediator, IPaginationService paginationService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IPaginationService _paginationService = paginationService;

        
        
        /// <summary>
        /// Lists all services with optional search and pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ServiceDto>), 200)]
        public async Task<IActionResult> ListServices(
            [FromQuery] string? nameSearchTerm,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new ListServicesQuery(nameSearchTerm)
            {
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific service by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetService(Guid id)
        {
            var query = new Spotless.Application.Features.Services.Queries.GetServiceById.GetServiceByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves featured services for homepage
        /// </summary>
        [HttpGet("featured")]
        [ProducesResponseType(typeof(IReadOnlyList<ServiceDto>), 200)]
        public async Task<IActionResult> GetFeaturedServices([FromQuery] int count = 4)
        {
            var query = new GetFeaturedServicesQuery(count);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new service (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
        {
            var adminId = GetCurrentUserId();
            var command = new CreateServiceCommand(dto, adminId);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetService), new { id = result }, result);
        }

        /// <summary>
        /// Updates an existing service (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceDto dto)
        {
            // Ensure the ServiceId in the DTO matches the route parameter
            var updateDto = dto with { ServiceId = id };
            var adminId = GetCurrentUserId();
            var command = new UpdateServiceCommand(updateDto, adminId);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes a service (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var command = new DeleteServiceCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new UnauthorizedAccessException("User identity claim is missing or invalid.");
            }
            return userId;
        }
    }
}

