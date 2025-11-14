using Spotless.Application.Dtos.Driver;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers;

public static class DriverMapper
{
    public static DriverProfileDto ToProfileDto(this Driver driver)
    {
        return new DriverProfileDto(
            Id: driver.Id,
            Name: driver.Name,
            Email: driver.Email,
            Phone: driver.Phone,
            VehicleInfo: driver.VehicleInfo,
            Status: driver.Status
        );
    }
}