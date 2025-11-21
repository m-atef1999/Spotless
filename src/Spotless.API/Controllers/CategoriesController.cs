using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Features.Categories.Commands.CreateCategory;
using Spotless.Application.Features.Categories.Commands.DeleteCategory;
using Spotless.Application.Features.Categories.Commands.UpdateCategory;
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

        /// <summary>
        /// Creates a new category
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var command = new CreateCategoryCommand(
                dto.Name,
                dto.Price,
                dto.Description
            );

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateCategory), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var command = new UpdateCategoryCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var command = new DeleteCategoryCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
