namespace Spotless.Application.Dtos.Service
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal EstimatedDurationHours { get; set; }
        public decimal MaxWeightKg { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
    }
}