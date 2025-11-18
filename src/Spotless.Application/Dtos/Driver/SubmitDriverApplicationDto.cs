namespace Spotless.Application.Dtos.Driver
{
    public record SubmitDriverApplicationDto
    {
        public string Email { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;


        public string Phone { get; init; } = string.Empty;

        public string VehicleInfo { get; init; } = string.Empty;
    }
}