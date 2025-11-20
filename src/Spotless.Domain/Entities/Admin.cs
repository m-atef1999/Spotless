using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class Admin : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public AdminRole AdminRole { get; private set; }

        public UserRole Role { get; private set; } = UserRole.Admin;

        private readonly List<Driver> _drivers = [];
        public IReadOnlyCollection<Driver> Drivers => _drivers.AsReadOnly();


        private readonly List<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();


        private readonly List<Payment> _payments = [];
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        protected Admin() { }

        public Admin(string name, string email, AdminRole adminrole) : base()
        {
            Name = name;
            Email = email;
            AdminRole = adminrole;
            this.Role = UserRole.Admin;
        }

    }
}
