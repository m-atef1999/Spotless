namespace Spotless.Application.Dtos.Driver
{
    public record DriverUpdateRequest(
        string Name,
        string? Phone,
        string VehicleInfo
    );
}
