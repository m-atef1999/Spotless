using Spotless.Application.Dtos.Service;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Mappers
{

    public class ServiceMapper : IServiceMapper
    {

        public ServiceDto MapToDto(Service service)
        {
            if (service == null) return null!;

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


        public IEnumerable<ServiceDto> MapToDto(IEnumerable<Service> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}