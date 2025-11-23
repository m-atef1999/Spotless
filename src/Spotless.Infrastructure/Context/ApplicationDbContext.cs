using Microsoft.EntityFrameworkCore;
using Audit.EntityFramework;
using Spotless.Domain.Entities;
using System.Reflection;

namespace Spotless.Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : AuditDbContext(options)
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<OrderDriverApplication> OrderDriverApplications => Set<OrderDriverApplication>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
        public DbSet<PaymentMethodEntity> PaymentMethods => Set<PaymentMethodEntity>();

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

                b.OwnsOne(c => c.WalletBalance, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("WalletBalance").HasColumnType("decimal(18,2)");
                    money.Property(m => m.Currency).HasColumnName("WalletCurrency").HasMaxLength(5);
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

                // Configure MaxWeightKg precision to avoid truncation warnings
                b.Property(s => s.MaxWeightKg).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Cart>(b =>
            {
                b.ToTable("Carts");
                b.HasKey(c => c.Id);
                b.Property(c => c.CustomerId).IsRequired();
                b.HasMany<CartItem>(c => c.Items).WithOne(i => i.Cart).HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(b =>
            {
                b.ToTable("CartItems");
                b.HasKey(i => i.Id);
                b.Property(i => i.ServiceId).IsRequired();
                b.Property(i => i.Quantity).IsRequired();
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.ToTable("AuditLogs");
                b.Property(a => a.EventType).HasMaxLength(200);
                b.Property(a => a.UserName).HasMaxLength(200);
                b.Property(a => a.IpAddress).HasMaxLength(45);
                b.Property(a => a.CorrelationId).HasMaxLength(100);
                b.Property(a => a.Data).HasColumnType("nvarchar(max)");
                b.Property(a => a.OccurredAt).HasColumnType("datetime2");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
