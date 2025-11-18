using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Driver
{

    public record DriverDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string VehicleInfo { get; init; } = string.Empty;

        public string Status { get; init; } = DriverStatus.Offline.ToString();

        public LocationDto? CurrentLocation { get; init; }
    }
}