using Microsoft.AspNetCore.Identity;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IAuthUser
    {
        public Guid? CustomerId { get; set; }
        public Guid? AdminId { get; set; }
        public Guid? DriverId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime LastLoginDate { get; set; }

        public virtual Customer? Customer { get; set; }

        public Address? Address { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        bool IAuthUser.EmailConfirmed => base.EmailConfirmed;
        string? IAuthUser.Email => base.Email;
        string? IAuthUser.UserName => base.UserName;
        Guid IAuthUser.Id => base.Id;


    }
}
