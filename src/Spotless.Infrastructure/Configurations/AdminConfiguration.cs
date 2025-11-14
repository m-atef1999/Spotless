using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasMany(a => a.Drivers)
                   .WithOne()
                   .HasForeignKey(d => d.AdminId)
                   .OnDelete(DeleteBehavior.SetNull);


            builder.HasMany(a => a.Orders)
                   .WithOne()
                   .HasForeignKey(o => o.AdminId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);


            builder.HasMany(a => a.Payments)
                   .WithOne()
                   .HasForeignKey(p => p.AdminId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
