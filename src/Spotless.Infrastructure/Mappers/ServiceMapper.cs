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

            return new ServiceDto
            {
                Id = service.Id,
                CategoryId = service.CategoryId,
                Name = service.Name,
                Description = service.Description,
                BasePrice = service.BasePrice.Amount,
                MaxWeightKg = service.MaxWeightKg,
                EstimatedDurationHours = service.EstimatedDurationHours,
                IsActive = service.IsActive,
                IsFeatured = service.IsFeatured
            };
        }

        public IEnumerable<ServiceDto> MapToDto(IEnumerable<Service> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}
