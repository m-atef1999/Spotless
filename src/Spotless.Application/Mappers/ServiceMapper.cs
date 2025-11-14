using Spotless.Application.Dtos.Service;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public static class ServiceMapper
    {

        public static ServiceDto ToDto(this Service service)
        {

            string categoryName = service.Category?.Name ?? "N/A";
            string categoryPrice = service.Category != null
                                 ? $"{service.Category.Price.Amount:N2} {service.Category.Price.Currency}"
                                 : "N/A";

            return new ServiceDto(
                Id: service.Id,
                CategoryId: service.CategoryId,
                CategoryName: categoryName,
                Name: service.Name,
                Description: service.Description,
                CategoryPrice: categoryPrice);
        }
    }
}
