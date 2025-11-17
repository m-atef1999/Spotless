namespace Spotless.Application.Dtos.Driver
{
    public record SubmitDriverApplicationDto(
        string VehicleInfo,
        string Name,
        string Phone,
        string Email
    );
}