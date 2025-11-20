using Spotless.Application.Dtos.Driver;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public interface IDriverMapper
    {
        DriverDto MapToProfileDto(Driver driver);

        IEnumerable<DriverDto> MapToProfileDto(IEnumerable<Driver> drivers);
    }
}
