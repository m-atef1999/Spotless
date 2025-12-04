using MediatR;
using Spotless.Application.Dtos.Category;

namespace Spotless.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageData { get; init; }
    }
}

