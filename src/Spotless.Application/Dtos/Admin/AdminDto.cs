namespace Spotless.Application.Dtos.Admin
{
    public record AdminDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string AdminRole { get; init; } = string.Empty;
    }
}
