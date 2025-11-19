using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Infrastructure.Configurations
{
    public class OrderDriverApplicationConfiguration : IEntityTypeConfiguration<OrderDriverApplication>
    {
        public void Configure(EntityTypeBuilder<OrderDriverApplication> builder)
        {
            builder.ToTable("OrderDriverApplications");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderId).IsRequired();
            builder.Property(o => o.DriverId).IsRequired();
            builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(o => o.AppliedAt).HasColumnType("datetime2").IsRequired();

            builder.HasOne<Driver>()
                .WithMany()
                .HasForeignKey("DriverId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}