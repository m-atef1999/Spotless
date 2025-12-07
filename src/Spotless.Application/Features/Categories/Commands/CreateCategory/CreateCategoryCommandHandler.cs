using MediatR;
using Spotless.Application.Dtos.Category;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category(
                request.Name,
                new Money(request.Price, "EGP"),
                request.Description,
                request.ImageUrl
            );

            await _unitOfWork.Categories.AddAsync(category);
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


