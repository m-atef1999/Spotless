using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

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
        public Location CurrentLocation { get; private set; } = null!;

        private readonly List<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        public UserRole Role { get; private set; } = UserRole.Driver;

        protected Driver() { }

        public Driver(Guid? adminId, string name, string email, string? phone, string vehicleInfo) : base()
        {
            AdminId = adminId;
            Name = name;
            Email = email;
            Phone = phone;
            VehicleInfo = vehicleInfo;
            Status = DriverStatus.Offline;
            this.Role = UserRole.Driver;
        }

        public double AverageRating { get; private set; }
        public int NumberOfReviews { get; private set; }

        public void UpdateRating(int newRating)
        {
            if (newRating < 1 || newRating > 5)
                throw new ArgumentOutOfRangeException(nameof(newRating), "Rating must be between 1 and 5.");

            double totalRating = (AverageRating * NumberOfReviews) + newRating;
            NumberOfReviews++;
            AverageRating = totalRating / NumberOfReviews;
        }

        public void UpdateStatus(DriverStatus newStatus)
        {
            Status = newStatus;
        }

        public void UpdateProfile(string name, string? phone, string vehicleInfo)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name cannot be empty.");
            if (string.IsNullOrWhiteSpace(vehicleInfo))
                throw new ArgumentException("Vehicle information is required.");

            Name = name;
            Phone = phone;
            VehicleInfo = vehicleInfo;
        }

        public void UpdateLocation(decimal lat, decimal lon)
        {
            CurrentLocation = new Location(lat, lon);
        }

        public void SetAdminId(Guid? adminId)
        {

            AdminId = adminId;
        }

        public void SetIdentityId(Guid userId)
        {
            this.Id = userId;
        }
    }
}
