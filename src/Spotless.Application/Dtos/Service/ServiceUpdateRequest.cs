namespace Spotless.Application.Dtos.Service
{
    public record UpdateServiceDto
    {

        public Guid ServiceId { get; init; }

        public string? Name { get; init; }
        public string? Description { get; init; }

        public decimal? PricePerUnitValue { get; init; }
        public string? PricePerUnitCurrency { get; init; }
        public decimal? EstimatedDurationHours { get; init; }
        public decimal? MaxWeightKg { get; init; }

        public Guid? CategoryId { get; init; }
        
        public string? ImageUrl { get; init; }
    }
}


