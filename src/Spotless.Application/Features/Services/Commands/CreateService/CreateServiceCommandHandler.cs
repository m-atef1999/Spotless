using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Services.Commands.CreateService
{
    public class CreateServiceCommandHandler(IUnitOfWork unitOfWork, CachedServiceService cachedServiceService) : IRequestHandler<CreateServiceCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly CachedServiceService _cachedServiceService = cachedServiceService;

        public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {

            var category = await _unitOfWork.Categories.GetByIdAsync(request.Dto.CategoryId) ?? throw new KeyNotFoundException($"Service Category with ID {request.Dto.CategoryId} not found.");
            var pricePerUnit = new Money(request.Dto.PricePerUnitAmount, request.Dto.PricePerUnitCurrency);


            var serviceEntity = new Service(
                categoryId: request.Dto.CategoryId,
                name: request.Dto.Name,
                description: request.Dto.Description,
                pricePerUnit: pricePerUnit,
                estimatedDurationHours: request.Dto.EstimatedDurationHours,
                maxWeightKg: request.Dto.MaxWeightKg
            );

            await _unitOfWork.Services.AddAsync(serviceEntity);
            await _unitOfWork.CommitAsync();
            
            // Invalidate cache
            await _cachedServiceService.InvalidateServiceCacheAsync();

            return serviceEntity.Id;
        }
    }
}
