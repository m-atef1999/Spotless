using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class DriverApplicationConfiguration : IEntityTypeConfiguration<DriverApplication>
    {
        public void Configure(EntityTypeBuilder<DriverApplication> builder)
        {
            builder.ToTable("DriverApplications");
            builder.HasKey(da => da.Id);

            builder.Property(da => da.CustomerId).IsRequired();
            
            // Store enum as string for consistency with other entities
            builder.Property(da => da.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();
            
            builder.Property(da => da.VehicleInfo)
                   .HasMaxLength(500)
                   .IsRequired();
            
            builder.Property(da => da.RejectionReason)
                   .HasMaxLength(1000);

            builder.HasOne(da => da.Customer)
                .WithMany()
                .HasForeignKey(da => da.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
