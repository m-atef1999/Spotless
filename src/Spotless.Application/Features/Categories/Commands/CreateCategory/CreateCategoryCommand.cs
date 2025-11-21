using MediatR;
using Spotless.Application.Dtos.Category;

namespace Spotless.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(
        string Name,
        decimal Price,
        string? Description) : IRequest<CategoryDto>;
}
