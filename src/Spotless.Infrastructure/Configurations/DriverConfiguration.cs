using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Email).IsRequired().HasMaxLength(256);
            builder.Property(d => d.VehicleInfo).HasMaxLength(500);


            builder.HasMany(d => d.Orders)
                   .WithOne()
                   .HasForeignKey(o => o.DriverId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
