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
            services.AddScoped<IPaginationService, PaginationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, DummySmsService>();
            services.AddScoped<IMessageSender>(sp =>
            {
                // Choose implementation based on configuration - default to Dummy for prototype
                var smsSettings = sp.GetRequiredService<IConfiguration>().GetSection(SmsSettings.SettingsKey).Get<SmsSettings>();
                if (smsSettings != null && smsSettings.Provider == "Twilio")
                {
                    return sp.GetRequiredService<TwilioMessageSender>();
                }

                return sp.GetRequiredService<DummyMessageSender>();
            });
            services.AddScoped<IOrderMapper, OrderMapper>();
            services.AddScoped<ICustomerMapper, CustomerMapper>();
            services.AddScoped<IServiceMapper, ServiceMapper>();
            services.AddScoped<IDriverMapper, DriverMapper>();

            // Domain Events and Services
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<INotificationService, NotificationService>();
            
            // Distributed Locking Service for concurrent operations (time-slot booking)
            services.AddScoped<IDistributedLockService, RedisDistributedLockService>();
            
            // Cached Services
            services.AddScoped<CachedAdminService>();
            services.AddScoped<CachedCustomerService>();
            services.AddScoped<CachedDriverService>();
            services.AddScoped<CachedCategoryService>();
            services.AddScoped<CachedServiceService>();
            services.AddScoped<CachedTimeSlotService>();
            
            // Configure Sms/WhatsApp settings from config
            services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SettingsKey));
            services.Configure<WhatsAppSettings>(configuration.GetSection(WhatsAppSettings.SettingsKey));
            
            // RabbitMQ Message Broker for decoupled background jobs
            services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SettingsKey));
            var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SettingsKey).Get<RabbitMqSettings>();
            if (rabbitMqSettings?.Enabled != false)
            {
                services.AddSingleton<IMessageBroker, RabbitMqMessageBroker>();
            }
            else
            {
                // Register a no-op implementation if RabbitMQ is disabled
                services.AddSingleton<IMessageBroker>(sp => new NoOpMessageBroker(sp.GetRequiredService<ILogger<NoOpMessageBroker>>()));
            }

            // Routing / ETA services
            services.Configure<RoutingSettings>(configuration.GetSection(RoutingSettings.SettingsKey));
            // Register MapboxRouterService with a typed HttpClient
            services.AddHttpClient<MapboxRouterService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            services.AddScoped<FallbackRouterService>();
            services.AddScoped<IRouterService>(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>().GetSection(RoutingSettings.SettingsKey).Get<RoutingSettings>() ?? new RoutingSettings();

                // Allow overriding the Mapbox API key via environment variable `MAPBOX_API_KEY`
                var configuration = sp.GetRequiredService<IConfiguration>();
                var envKey = configuration["MAPBOX_API_KEY"] ?? configuration.GetSection("Routing")?["MapboxApiKey"];
                if (!string.IsNullOrEmpty(envKey))
                {
                    cfg.MapboxApiKey = envKey;
                }

                if (cfg != null && !string.IsNullOrEmpty(cfg.Provider) && cfg.Provider.Equals("Mapbox", StringComparison.OrdinalIgnoreCase))
                {
                    // If key looks like the placeholder or is missing, fall back to the simple estimator
                    if (string.IsNullOrEmpty(cfg.MapboxApiKey) || cfg.MapboxApiKey.Contains("PLACEHOLDER") || cfg.MapboxApiKey.StartsWith("pk.test"))
                    {
                        return sp.GetRequiredService<FallbackRouterService>();
                    }

                    return sp.GetRequiredService<MapboxRouterService>();
                }

                return sp.GetRequiredService<FallbackRouterService>();
            });

            // Register Twilio sender as an option alongside Dummy; selection can be changed in DI later
            services.AddScoped<TwilioMessageSender>();
            services.AddScoped<DummyMessageSender>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ICacheService, CacheService>();

            // Caching Services
            services.AddScoped<ICachingService, DistributedCachingService>();
            services.AddScoped<CachedServiceService>();
            services.AddScoped<CachedCategoryService>();
            services.AddScoped<CachedTimeSlotService>();

            // Cart services and repositories
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();


            // Payment Services
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
            services.AddScoped<IPaymobSignatureService, PaymobSignatureService>();

            // Bind Sms/WhatsApp configuration to DI
            services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SettingsKey));
            services.Configure<WhatsAppSettings>(configuration.GetSection(WhatsAppSettings.SettingsKey));

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
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IOrderDriverApplicationRepository, OrderDriverApplicationRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

            return services;
        }
    }
}
