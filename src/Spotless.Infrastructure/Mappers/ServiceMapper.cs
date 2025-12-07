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
                BasePrice = service.BasePrice.Amount > 0 ? service.BasePrice.Amount : service.PricePerUnit.Amount,
                PricePerUnit = service.PricePerUnit.Amount,
                Currency = !string.IsNullOrEmpty(service.BasePrice.Currency) ? service.BasePrice.Currency : service.PricePerUnit.Currency,
                MaxWeightKg = service.MaxWeightKg,
                EstimatedDurationHours = service.EstimatedDurationHours,
                IsActive = service.IsActive,
                IsFeatured = service.IsFeatured,
                ImageUrl = service.ImageUrl
            };
        }

        public IEnumerable<ServiceDto> MapToDto(IEnumerable<Service> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}
