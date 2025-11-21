namespace Spotless.Application.Dtos.Category
{
    public record CategoryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public int ServiceCount { get; init; }
    }
}
