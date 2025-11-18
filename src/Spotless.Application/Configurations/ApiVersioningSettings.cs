namespace Spotless.Application.Configurations
{
    public class ApiVersioningSettings
    {
        public const string SettingsKey = "ApiVersioningSettings";

        public string DefaultVersion { get; set; } = "v1";
        public string[] SupportedVersions { get; set; } = new[] { "v1" };
        public bool RequireVersionInPath { get; set; } = false;
    }
}

