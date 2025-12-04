namespace Spotless.Application.Dtos.Service
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal EstimatedDurationHours { get; set; }
        public decimal MaxWeightKg { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageData { get; set; }
    }
}

