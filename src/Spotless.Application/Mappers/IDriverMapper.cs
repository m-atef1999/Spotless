using Spotless.Application.Dtos.Driver;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public interface IDriverMapper
    {

        DriverProfileDto MapToProfileDto(Driver entity);


        IEnumerable<DriverProfileDto> MapToProfileDto(IEnumerable<Driver> entities);
    }
}