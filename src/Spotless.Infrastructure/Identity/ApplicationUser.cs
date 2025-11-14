using Microsoft.AspNetCore.Identity;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;

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
    }
}
