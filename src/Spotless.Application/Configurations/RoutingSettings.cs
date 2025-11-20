namespace Spotless.Application.Configurations
{
    public class RoutingSettings
    {
        public const string SettingsKey = "Routing";
        public string Provider { get; set; } = "Mapbox";
        public string MapboxApiKey { get; set; } = string.Empty;
        public double DefaultAverageSpeedKph { get; set; } = 30.0;
    }
}
