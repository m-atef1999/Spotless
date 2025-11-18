using Audit.Core;
using Audit.EntityFramework;
using Audit.WebApi;
using Serilog;
using Spotless.API.Extensions;
using Spotless.API.Middleware;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Configurations;
using Spotless.Infrastructure.SeedData;
using Spotless.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();



Audit.Core.Configuration.Setup()
    .UseSqlServer(config => config
        .ConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .TableName("AuditEvents")
        .IdColumnName("EventId")
        .JsonColumnName("EventData")
        .LastUpdatedColumnName("LastUpdatedDate")
    );



Audit.Core.Configuration.AddCustomAction(ActionType.OnEventSaving, scope =>
{
    try
    {
        var ev = scope.Event;

        var ef = ev?.GetEntityFrameworkEvent();
        if (ef?.Entries != null)
        {
            foreach (var entry in ef.Entries)
            {
                if (entry.Changes != null)
                {
                    foreach (var ch in entry.Changes)
                    {
                        var col = ch.ColumnName ?? string.Empty;
                        var low = col.ToLowerInvariant();
                        if (low.Contains("password") || low.Contains("card") || low.Contains("cvv") || low.Contains("ssn"))
                        {
                            ch.NewValue = "***REDACTED***";
                            ch.OriginalValue = "***REDACTED***";
                        }
                    }
                }
            }
        }


        try
        {
            if (ev?.CustomFields != null && ev.CustomFields.ContainsKey("RequestBody"))
            {
                var rb = ev.CustomFields["RequestBody"] as string;
                if (!string.IsNullOrEmpty(rb))
                {
                    ev.CustomFields["RequestBody"] = Spotless.API.Utils.AuditRedactor.RedactJson(rb);
                }
            }
        }
        catch { }

    }
    catch { }
});

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
builder.Services.Configure<ApiVersioningSettings>(
    builder.Configuration.GetSection(ApiVersioningSettings.SettingsKey));


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

// 1. Request/Response Logging (first - to log all requests and responses)
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 2. Global Exception Handling (catches all exceptions from downstream middleware)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// 3. API Versioning (to handle version routing)
app.UseMiddleware<ApiVersioningMiddleware>();

// 4. HTTPS Redirection
app.UseHttpsRedirection();

// 5. CORS (must come before authentication)
if (securitySettings?.Cors?.EnableCors == true)
{
    app.UseCors("DefaultCorsPolicy");
}

// 6. Rate Limiting
var rateLimitSettings = securitySettings?.RateLimit;
if (rateLimitSettings?.EnableRateLimit == true)
{
    app.UseMiddleware<RateLimitingMiddleware>();
}

// 7. HTTPS Enforcement
if (securitySettings?.EnforceHttps == true)
{
    app.UseMiddleware<HttpsEnforcementMiddleware>();
}

// 8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Audit middleware should run after authentication so user info is available
app.UseAuditMiddleware(config =>
{
    // You can add further specific configurations here, 
    // such as ignoring certain paths or response codes.
    // e.g., config.FilterResponse(c => c.HttpStatusCode == 200);
});

// 9. Controllers
app.MapControllers();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}