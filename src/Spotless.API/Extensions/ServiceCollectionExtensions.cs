using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spotless.Application.Behaviors;
using Spotless.Application.Configurations;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Markers;
using Spotless.Application.Services;
using Spotless.Application.Validation;
using Spotless.Infrastructure.Context;
using Spotless.Infrastructure.Identity;
using Spotless.Infrastructure.Mappers;
using Spotless.Infrastructure.Repositories;
using Spotless.Infrastructure.Services;
using System.Text;

namespace Spotless.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Spotless.Infrastructure")
                )
            );

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"))
            );

            return services;
        }

        public static IServiceCollection AddIdentityAndAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SettingsKey));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection(JwtSettings.SettingsKey).Get<JwtSettings>();

                if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
                    throw new InvalidOperationException("JwtSettings.Secret is not configured.");

                var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(OrderDto).Assembly)
                   .AddOpenBehavior(typeof(TransactionBehavior<,>));
            });
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

            services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();

            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, DummySmsService>();
            services.AddScoped<IOrderMapper, OrderMapper>();
            services.AddScoped<ICustomerMapper, CustomerMapper>();
            services.AddScoped<IServiceMapper, ServiceMapper>();
            services.AddScoped<IDriverMapper, DriverMapper>();

            // Domain Events and Services
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ICacheService, CacheService>();

            // Caching Services
            services.AddScoped<ICachingService, DistributedCachingService>();
            services.AddScoped<CachedServiceService>();
            services.AddScoped<CachedCategoryService>();
            services.AddScoped<CachedTimeSlotService>();

            // Payment Services
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
            services.AddScoped<IPaymobSignatureService, PaymobSignatureService>();

            // Configure Paymob settings (binds to DI)

            services.Configure<PaymobSettings>(configuration.GetSection(PaymobSettings.SettingsKey));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IOrderDriverApplicationRepository, OrderDriverApplicationRepository>();

            return services;
        }
    }
}
