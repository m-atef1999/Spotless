using Spotless.Application.Dtos.Service;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public static class ServiceMapper
    {
        public static ServiceDto ToDto(this Service service)
        {
            return new ServiceDto(
                Id: service.Id,
                CategoryId: service.CategoryId,
                Name: service.Name,
                Description: service.Description,
                PricePerUnitAmount: service.PricePerUnit.Amount,
                PricePerUnitCurrency: service.PricePerUnit.Currency,
                EstimatedDurationHours: service.EstimatedDurationHours
            );
        }
    }
}
