namespace Spotless.Application.Configurations
{
    public class SecuritySettings
    {
        public const string SettingsKey = "SecuritySettings";

        public bool EnforceHttps { get; set; } = true;
        
        public CorsSettings Cors { get; set; } = new();
        
        public RateLimitSettings RateLimit { get; set; } = new();
    }

    public class CorsSettings
    {
        public bool EnableCors { get; set; } = true;
        public string[] AllowedOrigins { get; set; } = [];
        public string[] AllowedMethods { get; set; } = ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"];
        public string[] AllowedHeaders { get; set; } = ["Content-Type", "Authorization"];
        public bool AllowCredentials { get; set; } = true;
    }

    public class RateLimitSettings
    {
        public bool EnableRateLimit { get; set; } = true;
        public int MaxRequests { get; set; } = 100;
        public int TimeWindowMinutes { get; set; } = 1;
    }
}

