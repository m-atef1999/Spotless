namespace Spotless.Application.Dtos
{

    public record LocationDto
    {
        public decimal Latitude { get; init; }
        public decimal Longitude { get; init; }
    }
}