namespace Spotless.Application.Configurations
{
    public class PaginationSettings
    {
        public const string SettingsKey = "PaginationSettings";

        public int DefaultPageNumber { get; set; } = 1;
        public int DefaultPageSize { get; set; } = 25;
        public int MaxPageSize { get; set; } = 100;
    }
}

