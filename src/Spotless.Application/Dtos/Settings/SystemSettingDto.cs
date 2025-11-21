namespace Spotless.Application.Dtos.Settings
{
    public record SystemSettingDto
    {
        public Guid Id { get; init; }
        public string Key { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime LastModified { get; init; }
    }

    public record UpdateSystemSettingDto
    {
        public string Value { get; init; } = string.Empty;
    }
}
