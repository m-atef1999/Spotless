using Microsoft.EntityFrameworkCore;
using Spotless.Domain.Entities;
using System.Reflection;

namespace Spotless.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Customer>(b =>
            {
                b.OwnsOne(c => c.Address, a =>
                {
                    a.Property(p => p.Street).HasColumnName("Street");
                    a.Property(p => p.City).HasColumnName("City");
                    a.Property(p => p.Country).HasColumnName("Country");
                    a.Property(p => p.ZipCode).HasColumnName("ZipCode");
                });
            });
            modelBuilder.Entity<Category>(b =>
            {

                b.OwnsOne(c => c.Price, p =>
                {
                    p.Property(m => m.Amount).HasColumnName("Price_Amount").HasColumnType("decimal(18,2)");
                    p.Property(m => m.Currency).HasColumnName("Price_Currency").HasMaxLength(3);
                });
            });


            modelBuilder.Entity<Service>(b =>
            {

                b.OwnsOne(s => s.BasePrice, p =>
                {
                    p.Property(m => m.Amount).HasColumnName("BasePrice_Amount").HasColumnType("decimal(18,2)");
                    p.Property(m => m.Currency).HasColumnName("BasePrice_Currency").HasMaxLength(3);
                });


                b.OwnsOne(s => s.PricePerUnit, p =>
                {
                    p.Property(m => m.Amount).HasColumnName("PricePerUnit_Amount").HasColumnType("decimal(18,2)");
                    p.Property(m => m.Currency).HasColumnName("PricePerUnit_Currency").HasMaxLength(3);
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
