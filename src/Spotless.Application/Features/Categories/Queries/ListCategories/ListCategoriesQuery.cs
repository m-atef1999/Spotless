using MediatR;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;

namespace Spotless.Application.Features.Categories.Queries.ListCategories
{
    public record ListCategoriesQuery : PaginationBaseRequest, IRequest<PagedResponse<CategoryDto>>;
}
