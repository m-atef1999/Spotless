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
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);

        }
    }
}