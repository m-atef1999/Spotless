using Spotless.API.Extensions;
using Spotless.API.Middleware;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Configurations;
using Spotless.Infrastructure.SeedData;
using Spotless.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDataProtection();

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddRepositories()
    .AddIdentityAndAuthentication(builder.Configuration)
    .AddApplicationServices();


builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SettingsKey));
builder.Services.Configure<PaymentGatewaySettings>(
    builder.Configuration.GetSection(PaymentGatewaySettings.SettingsKey));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SettingsKey));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SettingsKey));
builder.Services.Configure<PaginationSettings>(
    builder.Configuration.GetSection(PaginationSettings.SettingsKey));
builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection(NotificationSettings.SettingsKey));
builder.Services.Configure<ReviewSettings>(
    builder.Configuration.GetSection(ReviewSettings.SectionName));
builder.Services.Configure<EncryptionSettings>(
    builder.Configuration.GetSection(EncryptionSettings.SettingsKey));
builder.Services.Configure<SecuritySettings>(
    builder.Configuration.GetSection(SecuritySettings.SettingsKey));


builder.Services.AddScoped<IEncryptionService, DataProtectionService>();

builder.Services.AddSingleton<IPaymentGatewayService, PaymentGatewayService>();


var securitySettings = builder.Configuration.GetSection(SecuritySettings.SettingsKey).Get<SecuritySettings>();
if (securitySettings?.Cors?.EnableCors == true)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultCorsPolicy", policy =>
        {
            var corsSettings = securitySettings.Cors;

            if (corsSettings.AllowedOrigins?.Length > 0)
            {
                policy.WithOrigins(corsSettings.AllowedOrigins);
            }
            else
            {

                if (builder.Environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins("https://yourdomain.com");
                }
            }

            policy.WithMethods(corsSettings.AllowedMethods ?? new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" });
            policy.WithHeaders(corsSettings.AllowedHeaders ?? new[] { "Content-Type", "Authorization" });

            if (corsSettings.AllowCredentials && corsSettings.AllowedOrigins?.Length > 0)
            {
                policy.AllowCredentials();
            }
        });
    });
}

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Attempting to seed database...");

try
{
    await DbInitializer.SeedAsync(app.Services);
    logger.LogInformation("Database seeding completed successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during database seeding.");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


if (securitySettings?.Cors?.EnableCors == true)
{
    app.UseCors("DefaultCorsPolicy");
}


var rateLimitSettings = securitySettings?.RateLimit;
if (rateLimitSettings?.EnableRateLimit == true)
{
    app.UseMiddleware<RateLimitingMiddleware>();
}

if (securitySettings?.EnforceHttps == true)
{
    app.UseMiddleware<HttpsEnforcementMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();