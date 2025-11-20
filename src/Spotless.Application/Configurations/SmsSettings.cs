namespace Spotless.Application.Configurations
{
    public class SmsSettings
    {
        public const string SettingsKey = "SmsSettings";

        public string Provider { get; set; } = "Dummy";
        public string TwilioAccountSid { get; set; } = string.Empty;
        public string TwilioAuthToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
    }
}
