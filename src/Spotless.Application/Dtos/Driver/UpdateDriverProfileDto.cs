namespace Spotless.Application.Dtos.Driver
{
    public record UpdateDriverProfileDto(
        string Name,
        string? Phone,
        string VehicleInfo
    );
}
