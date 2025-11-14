using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid? AdminId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public Address Address { get; private set; } = null!;
        public Location DefaultLocation { get; private set; } = null!;
        public Money WalletBalance { get; private set; } = new Money(0, "EGP");
        public CustomerType Type { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Customer;

        public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
        public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

        protected Customer() { }

        public Customer(Guid? adminId, string name, string? phone, string email, Address address, CustomerType type) : base()
        {
            AdminId = adminId;
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Type = type;
            Role = UserRole.Customer;
        }

        public void UpdateProfile(string name, string? phone, Address address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be empty.");

            Name = name;
            Phone = phone;
            Address = address;
        }
    }
}
