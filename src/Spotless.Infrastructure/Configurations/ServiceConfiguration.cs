using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");
            builder.HasKey(s => s.Id);


            builder.HasOne(s => s.Category)
                    .WithMany(c => c.Services)
                    .HasForeignKey(s => s.CategoryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(s => s.BasePrice, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("BasePriceAmount")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("BasePriceCurrency")
                     .HasMaxLength(5)
                     .IsRequired();
            });


            builder.OwnsOne(s => s.PricePerUnit, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("PricePerItem_Amount")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("PricePerItem_Currency")
                     .HasMaxLength(5)
                     .IsRequired();
            });


            builder.Property(s => s.Name).HasMaxLength(256).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(1000).IsRequired();
            builder.Property(s => s.EstimatedDurationHours).HasColumnType("decimal(4,2)").IsRequired();


            builder.HasIndex(s => s.CategoryId).HasDatabaseName("IX_Service_CategoryId");
        }
    }
}

