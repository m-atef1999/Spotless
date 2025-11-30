using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Email)
                    .IsRequired()
                    .HasMaxLength(256);

            builder.HasIndex(a => a.Email)
                   .IsUnique()
                   .HasDatabaseName("IX_Admin_Email_Unique");



            builder.Property(a => a.Role)
                   .HasConversion<string>();

            builder.Property(a => a.AdminRole)
                   .HasConversion<string>();



            builder.HasMany(a => a.Orders)
                    .WithOne()
                    .HasForeignKey(o => o.AdminId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
