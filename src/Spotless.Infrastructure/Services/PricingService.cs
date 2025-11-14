using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Services
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Money> GetBasePriceAsync(Guid serviceId)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(serviceId);

            if (service == null)
            {
                throw new KeyNotFoundException($"Service with ID {serviceId} not found.");
            }

            return service.PricePerUnit;
        }
    }
}
