using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);


            builder.HasIndex(c => c.Email)
                   .IsUnique()
                   .HasDatabaseName("IX_Customer_Email_Unique");


            builder.HasIndex(c => c.AdminId)
                   .HasDatabaseName("IX_Customer_AdminId");


            builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200);

            builder.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").IsRequired();
                address.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            });

            builder.OwnsOne(c => c.DefaultLocation, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("DefaultLatitude").HasColumnType("decimal(10,8)");
                location.Property(l => l.Longitude).HasColumnName("DefaultLongitude").HasColumnType("decimal(11,8)");
            });

            builder.OwnsOne(c => c.WalletBalance, money =>
            {
                money.Property(m => m.Amount).HasColumnName("WalletBalance").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("WalletCurrency").HasMaxLength(5);
            });

            builder.HasMany(c => c.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Admin>()
                    .WithMany()
                    .HasForeignKey(c => c.AdminId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
