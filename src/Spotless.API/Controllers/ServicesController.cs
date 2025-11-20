using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Features.Services.Queries.GetFeaturedServices;
using Spotless.Application.Features.Services.Queries.ListAllServices;
using Spotless.Application.Interfaces;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController(IMediator mediator, IPaginationService paginationService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IPaginationService _paginationService = paginationService;

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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetService(Guid id)
        {
            var query = new Spotless.Application.Features.Services.Queries.GetServiceById.GetServiceByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("featured")]
        [ProducesResponseType(typeof(IReadOnlyList<ServiceDto>), 200)]
        public async Task<IActionResult> GetFeaturedServices([FromQuery] int count = 4)
        {
            var query = new GetFeaturedServicesQuery(count);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
