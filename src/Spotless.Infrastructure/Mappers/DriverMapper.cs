using Spotless.Application.Dtos;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Mappers
{
    public class DriverMapper : IDriverMapper
    {

        public DriverDto MapToProfileDto(Driver driver)
        {
            var locationDto = driver.CurrentLocation != null
                              && driver.CurrentLocation.Latitude.HasValue
                              && driver.CurrentLocation.Longitude.HasValue
                ? new LocationDto
                {
                    Latitude = driver.CurrentLocation.Latitude.Value,
                    Longitude = driver.CurrentLocation.Longitude.Value
                }
                : null;

            return new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                Email = driver.Email,
                Phone = driver.Phone,
                VehicleInfo = driver.VehicleInfo,
                Status = driver.Status.ToString(),
                UpdatedAt = driver.UpdatedAt,
                CurrentLocation = locationDto
            };
        }


        public IEnumerable<DriverDto> MapToProfileDto(IEnumerable<Driver> drivers)
        {
            return drivers.Select(MapToProfileDto);
        }
    }
}
