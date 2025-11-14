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
                money.Property(m => m.Amount)
                     .HasColumnName("PaymentAmount")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("PaymentCurrency")
                     .HasMaxLength(5)
                     .IsRequired();
            });

            builder.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.Customer)
                   .WithMany(c => c.Payments)
                   .HasForeignKey(p => p.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Admin)
                   .WithMany(a => a.Payments)
                   .HasForeignKey(p => p.AdminId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);


            builder.HasIndex(p => p.CustomerId).HasDatabaseName("IX_Payment_CustomerId");

            builder.HasIndex(p => p.OrderId)
                   .IsUnique(false)
                   .HasDatabaseName("IX_Payment_TransactionId");

            builder.HasIndex(p => p.AdminId).IsUnique(false).HasDatabaseName("IX_Payment_AdminId");
        }
    }

}
