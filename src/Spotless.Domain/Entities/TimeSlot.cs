namespace Spotless.Domain.Entities
{
    public class TimeSlot : BaseEntity
    {

        public string Name { get; private set; } = string.Empty;
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public int MaxCapacity { get; private set; }

        public string ValidDaysOfWeek { get; private set; } = string.Empty;


        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        protected TimeSlot() { }

        public TimeSlot(string name, TimeSpan startTime, TimeSpan endTime, int maxCapacity, string validDaysOfWeek) : base()
        {
            if (startTime >= endTime)
                throw new InvalidOperationException("Start time must be before end time.");
            if (maxCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Capacity must be greater than zero.");

            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            MaxCapacity = maxCapacity;
            ValidDaysOfWeek = validDaysOfWeek;
        }
    }
}
