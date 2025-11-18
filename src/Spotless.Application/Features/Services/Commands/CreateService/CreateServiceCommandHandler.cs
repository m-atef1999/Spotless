using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Services.Commands.CreateService
{
    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {

            var category = await _unitOfWork.Categories.GetByIdAsync(request.Dto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Service Category with ID {request.Dto.CategoryId} not found.");
            }


            var pricePerUnit = new Money(request.Dto.PricePerUnitAmount, request.Dto.PricePerUnitCurrency);


            var serviceEntity = new Service(
                categoryId: request.Dto.CategoryId,
                name: request.Dto.Name,
                description: request.Dto.Description,
                pricePerUnit: pricePerUnit,
                estimatedDurationHours: request.Dto.EstimatedDurationHours
            );

            await _unitOfWork.Services.AddAsync(serviceEntity);
            await _unitOfWork.CommitAsync();

            return serviceEntity.Id;
        }
    }
}