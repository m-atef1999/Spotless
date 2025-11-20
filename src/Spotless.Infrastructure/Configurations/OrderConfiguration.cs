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

            builder.OwnsOne(o => o.PickupLocation, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("PickupLatitude").HasColumnType("decimal(10,8)").IsRequired();
                location.Property(l => l.Longitude).HasColumnName("PickupLongitude").HasColumnType("decimal(11,8)").IsRequired();
            });


            builder.OwnsOne(o => o.DeliveryLocation, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("DeliveryLatitude").HasColumnType("decimal(10,8)").IsRequired();
                location.Property(l => l.Longitude).HasColumnName("DeliveryLongitude").HasColumnType("decimal(11,8)").IsRequired();
            });


            builder.HasIndex(o => o.CustomerId).HasDatabaseName("IX_Order_CustomerId");


            builder.HasIndex(o => o.DriverId).IsUnique(false).HasDatabaseName("IX_Order_DriverId");


            builder.HasIndex(o => new { o.ScheduledDate, o.TimeSlotId }).HasDatabaseName("IX_Order_Scheduled");


            builder.HasIndex(o => o.Status).HasDatabaseName("IX_Order_Status");



            builder.HasOne<Driver>()
                    .WithMany(d => d.Orders)
                    .HasForeignKey(o => o.DriverId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
