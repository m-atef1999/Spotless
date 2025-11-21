using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            // Check if category has services
            if (category.Services.Any())
            {
                throw new InvalidOperationException("Cannot delete category with existing services");
            }

            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
