using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Interfaces
{
    public interface IPricingService
    {
        Task<Money> GetBasePriceAsync(Guid serviceId);
    }
}
