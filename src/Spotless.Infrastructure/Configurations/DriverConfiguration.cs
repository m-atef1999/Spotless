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

            builder.OwnsOne(d => d.CurrentLocation, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("CurrentLatitude").HasColumnType("decimal(10,8)").IsRequired(false);
                location.Property(l => l.Longitude).HasColumnName("CurrentLongitude").HasColumnType("decimal(11,8)").IsRequired(false);
            });

            builder.Property(d => d.AverageRating)
                   .HasDefaultValue(0.0);

            builder.Property(d => d.NumberOfReviews)
                   .HasDefaultValue(0);

            builder.Property(d => d.Email)
                    .IsRequired()
                    .HasMaxLength(256);

            builder.HasIndex(d => d.Email)
                   .IsUnique()
                   .HasDatabaseName("IX_Driver_Email_Unique");


            builder.HasIndex(d => d.AdminId)
                   .HasDatabaseName("IX_Driver_AdminId");



            builder.Property(d => d.Role)
                   .HasConversion<string>();


            builder.HasOne<Admin>()
                    .WithMany(a => a.Drivers)
                    .HasForeignKey(d => d.AdminId)
                    .IsRequired(false);
        }
    }
}
