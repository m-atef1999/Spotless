using Spotless.Application.Dtos.Driver;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Mappers
{
    public class DriverMapper : IDriverMapper
    {
        public DriverProfileDto MapToProfileDto(Driver driver)
        {
            if (driver == null) return null!;

            return new DriverProfileDto(
                Id: driver.Id,
                Name: driver.Name,
                Email: driver.Email,
                Phone: driver.Phone,
                VehicleInfo: driver.VehicleInfo,
                Status: driver.Status
            );
        }

        public IEnumerable<DriverProfileDto> MapToProfileDto(IEnumerable<Driver> entities)
        {
            return entities.Select(MapToProfileDto);
        }
    }
}