using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class Driver : BaseEntity
    {
        public Guid? AdminId { get; private set; }

        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string VehicleInfo { get; private set; } = string.Empty;
        public DriverStatus Status { get; private set; } = DriverStatus.Offline;

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        protected Driver() { }

        public Driver(Guid? adminId, string name, string email, string? phone, string vehicleInfo) : base()
        {
            AdminId = adminId;
            Name = name;
            Email = email;
            Phone = phone;
            VehicleInfo = vehicleInfo;
            Status = DriverStatus.Offline;
        }

        public void UpdateStatus(DriverStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
