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

                PricePerUnit: service.PricePerUnit,
                EstimatedDurationHours: service.EstimatedDurationHours
            );
        }

        public IEnumerable<ServiceDto> MapToDto(IEnumerable<Service> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}