using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);

            builder.OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PaymentAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("PaymentCurrency").HasMaxLength(5);
            });

            builder.HasOne(p => p.Customer)
                   .WithMany()
                   .HasForeignKey(p => p.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
