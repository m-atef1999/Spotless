using Microsoft.EntityFrameworkCore;
using Spotless.Data.Context;

namespace Spotless.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SpotlessDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Spotless.Data")
                )
            );

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(Spotless.Domain.Entities.BaseEntity).Assembly
                )
            );

            // Add FluentValidation validators when ready
            // services.AddValidatorsFromAssemblyContaining<SomeValidator>();

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            // Repository registrations will be added here
            // Example: services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}
