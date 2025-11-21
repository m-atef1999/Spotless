namespace Spotless.Application.Dtos.Category
{
    public record CreateCategoryDto
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Price { get; init; }
    }
}
