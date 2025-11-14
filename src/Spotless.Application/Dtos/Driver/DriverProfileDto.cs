using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Driver
{
    public record DriverProfileDto(
        Guid Id,
        string Name,
        string Email,
        string? Phone,
        string VehicleInfo,
        DriverStatus Status
    );
}
