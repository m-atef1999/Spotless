namespace Spotless.Application.Dtos.Review
{

    public record ReviewDto
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public Guid OrderId { get; init; }
        public Guid? DriverId { get; init; }

        public int Rating { get; init; }
        public string? Comment { get; init; }
    }
}