namespace Spotless.Infrastructure.Configurations
{
    public class DatabaseSettings
    {

        public const string SettingsKey = "Database";

        public string ConnectionString { get; set; } = string.Empty;
    }
}
