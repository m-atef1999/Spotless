using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(o => o.Id);


            builder.HasOne(o => o.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(o => o.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(o => o.Service)
                .WithMany()
                .HasForeignKey(o => o.ServiceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.OwnsOne(o => o.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PriceAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(5).IsRequired();
            });


            builder.HasIndex(o => o.OrderId).HasDatabaseName("IX_OrderItem_OrderId");
            builder.HasIndex(o => o.ServiceId).HasDatabaseName("IX_OrderItem_ServiceId");
        }
    }
}
