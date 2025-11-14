using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.OwnsOne(o => o.TotalPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(5).IsRequired();
            });

            builder.HasIndex(o => o.CustomerId);
            builder.HasIndex(o => o.DriverId).IsUnique(false);


            builder.HasOne<Driver>()
                   .WithMany(d => d.Orders)
                   .HasForeignKey(o => o.DriverId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Service>()
                   .WithMany()
                   .HasForeignKey(o => o.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
