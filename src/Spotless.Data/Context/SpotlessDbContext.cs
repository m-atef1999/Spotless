using Microsoft.EntityFrameworkCore;


namespace Spotless.Data.Context
{
    public class SpotlessDbContext : DbContext
    {
        public SpotlessDbContext(DbContextOptions options)
            : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpotlessDbContext).Assembly);
        }
    }
}
