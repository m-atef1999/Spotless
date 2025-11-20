namespace Spotless.Application.Configurations
{
    public class NotificationSettings
    {
        public const string SettingsKey = "NotificationSettings";

        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = true;
        public bool EnableWhatsAppNotifications { get; set; } = true;
        public bool EnablePushNotifications { get; set; } = false;


        public bool NotifyOnOrderCreated { get; set; } = true;
        public bool NotifyOnOrderConfirmed { get; set; } = true;
        public bool NotifyOnOrderCompleted { get; set; } = true;
        public bool NotifyOnOrderCancelled { get; set; } = true;

        public bool NotifyOnDriverAssigned { get; set; } = true;

        public bool NotifyOnPaymentCompleted { get; set; } = true;
        public bool NotifyOnPaymentFailed { get; set; } = true;
    }
}

