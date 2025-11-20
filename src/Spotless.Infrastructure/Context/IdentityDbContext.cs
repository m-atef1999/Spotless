using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotless.Infrastructure.Identity;

namespace Spotless.Infrastructure.Context
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Identity");

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");


                b.OwnsOne(u => u.Address, a =>
                {
                    a.Property(p => p.Street).HasColumnName("Street");
                    a.Property(p => p.City).HasColumnName("City");
                    a.Property(p => p.Country).HasColumnName("Country");
                    a.Property(p => p.ZipCode).HasColumnName("ZipCode");
                });
            });

            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");


            builder.Ignore<Domain.Entities.Customer>();
        }
    }
}
