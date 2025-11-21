using MediatR;

namespace Spotless.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }
}
