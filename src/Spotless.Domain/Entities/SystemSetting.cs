namespace Spotless.Domain.Entities
{
    public class SystemSetting : BaseEntity
    {
        public string Key { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime LastModified { get; private set; } = DateTime.UtcNow;

        protected SystemSetting() { }

        public SystemSetting(string key, string value, string category, string description)
        {
            Key = key;
            Value = value;
            Category = category;
            Description = description;
            LastModified = DateTime.UtcNow;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            LastModified = DateTime.UtcNow;
        }
    }
}
