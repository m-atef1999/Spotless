using MediatR;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            category.UpdateDetails(
                request.Name,
                new Money(request.Price, "EGP"),
                request.Description,
                request.ImageUrl
            );

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.CommitAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Price = category.Price.Amount,
                ServiceCount = category.Services.Count,
                ImageUrl = category.ImageUrl
            };
        }
    }
}


