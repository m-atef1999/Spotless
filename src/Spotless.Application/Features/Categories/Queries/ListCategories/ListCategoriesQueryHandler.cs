using MediatR;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Categories.Queries.ListCategories
{
    public class ListCategoriesQueryHandler(CachedCategoryService cachedCategoryService)
        : IRequestHandler<ListCategoriesQuery, PagedResponse<CategoryDto>>
    {
        private readonly CachedCategoryService _cachedCategoryService = cachedCategoryService;

        public async Task<PagedResponse<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
        {
            var cachedCategories = await _cachedCategoryService.GetAllCategoriesAsync();

            // Convert Category entities to CategoryDto
            var categoryDtos = cachedCategories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price.Amount,
                ServiceCount = c.Services?.Count ?? 0,
                ImageUrl = c.ImageUrl
            }).ToList();

            // Apply pagination
            var pagedCategories = categoryDtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<CategoryDto>(
                pagedCategories,
                categoryDtos.Count,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
