using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Features.Categories.Queries.ListCategories;
using Spotless.Application.Interfaces;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController(IMediator mediator, IPaginationService paginationService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IPaginationService _paginationService = paginationService;

        /// <summary>
        /// Lists all service categories with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CategoryDto>), 200)]
        public async Task<IActionResult> ListCategories(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new ListCategoriesQuery
            {
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
