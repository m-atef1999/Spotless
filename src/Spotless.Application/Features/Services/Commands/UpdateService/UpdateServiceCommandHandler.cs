using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Services.Commands.UpdateService
{
    public class UpdateServiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateServiceCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(request.Dto.ServiceId) ?? throw new KeyNotFoundException($"Service with ID {request.Dto.ServiceId} not found.");
            if (request.Dto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.Dto.CategoryId.Value) ?? throw new KeyNotFoundException($"Service Category with ID {request.Dto.CategoryId.Value} not found.");
            }


            Money? newPricePerUnit = null;
            if (request.Dto.PricePerUnitValue.HasValue || request.Dto.PricePerUnitCurrency != null)
            {

                decimal amount = request.Dto.PricePerUnitValue ?? service.PricePerUnit.Amount;
                string currency = request.Dto.PricePerUnitCurrency ?? service.PricePerUnit.Currency;

                if (amount <= 0)
                {
                    throw new InvalidOperationException("Price per unit must be greater than zero.");
                }

                newPricePerUnit = new Money(amount, currency);
            }


            service.Update(
                name: request.Dto.Name,
                description: request.Dto.Description,
                pricePerUnit: newPricePerUnit,
                estimatedDurationHours: request.Dto.EstimatedDurationHours,
                categoryId: request.Dto.CategoryId,
                maxWeightKg: request.Dto.MaxWeightKg
            );


            await _unitOfWork.Services.UpdateAsync(service);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
