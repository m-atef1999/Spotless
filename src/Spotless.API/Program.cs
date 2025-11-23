using Audit.Core;
using Audit.EntityFramework;
using Audit.WebApi;
using Microsoft.OpenApi.Models;
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



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();


builder.Services.AddDataProtection();
builder.Services.AddSignalR();


builder.Configuration
       .SetBasePath(builder.Environment.ContentRootPath)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
       .AddEnvironmentVariables();


// paymob settings with environment variable overrides

var paymobSettings = new Spotless.Application.Configurations.PaymobSettings();
builder.Configuration.GetSection(Spotless.Application.Configurations.PaymobSettings.SettingsKey).Bind(paymobSettings);
paymobSettings.ApiKey = Environment.GetEnvironmentVariable("Paymob__ApiKey") ?? paymobSettings.ApiKey;
paymobSettings.SecretKey = Environment.GetEnvironmentVariable("Paymob__SecretKey") ?? paymobSettings.SecretKey;
paymobSettings.HmacSecret = Environment.GetEnvironmentVariable("Paymob__HmacSecret") ?? paymobSettings.HmacSecret;
paymobSettings.PublicKey = Environment.GetEnvironmentVariable("Paymob__PublicKey") ?? paymobSettings.PublicKey;
builder.Services.Configure<Spotless.Application.Configurations.PaymobSettings>(
builder.Configuration.GetSection(Spotless.Application.Configurations.PaymobSettings.SettingsKey));

// Add Redis/IDistributedCache
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddRepositories()
    .AddIdentityAndAuthentication(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

// Real-time notifier (SignalR) implementation
builder.Services.AddScoped<Spotless.Application.Interfaces.IRealTimeNotifier, Spotless.API.Services.SignalRRealTimeNotifier>();
builder.Services.AddScoped<Spotless.Application.Interfaces.IPushNotificationSender, Spotless.API.Services.SignalRPushNotificationSender>();

// AI Service
builder.Services.AddScoped<Spotless.API.Services.IAiService, Spotless.API.Services.AiService>();

// Background Job Processor (hosted service for message queue consumption)
builder.Services.AddHostedService<BackgroundJobProcessor>();

// Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Spotless.Application.Interfaces.ICurrentUserService, Spotless.Infrastructure.Services.CurrentUserService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Spotless API",
        Version = "v1",
        Description = "Laundry and dry cleaning service API"
    });
    // Include XML comments for API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Use full name for schema IDs to avoid conflicts
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));



    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

    { Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" });



    options.AddSecurityRequirement(new OpenApiSecurityRequirement

    {
        { new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme,Id = "Bearer"} }, Array.Empty<string>() }

    });

});



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



var securitySettings = builder.Configuration.GetSection(SecuritySettings.SettingsKey).Get<SecuritySettings>();
if (securitySettings?.Cors?.EnableCors == true)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultCorsPolicy", policy =>
        {
            var corsSettings = securitySettings.Cors;
            
            // Combine config origins with hardcoded defaults to ensure connectivity
            var origins = new List<string> 
            { 
                "http://localhost:5173", 
                "https://spotless.runasp.net",
                "https://spotless-project.vercel.app"
            };

            if (corsSettings.AllowedOrigins?.Length > 0)
            {
                origins.AddRange(corsSettings.AllowedOrigins);
            }

            policy.WithOrigins(origins.Distinct().ToArray())
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
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


app.UseSwagger();
app.UseSwaggerUI();

// 1. CORS (Moved to top to handle preflights/redirects correctly)
app.UseCors("DefaultCorsPolicy");

// 2. Request/Response Logging
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 3. Global Exception Handling
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// 4. API Versioning
app.UseMiddleware<ApiVersioningMiddleware>();

// 5. HTTPS Redirection
app.UseHttpsRedirection();

// 6. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
    options.ApiPath = "/healthchecks-api";
});

app.MapHub<Spotless.API.Hubs.NotificationHub>("/notificationHub");
app.MapHub<Spotless.API.Hubs.DriverHub>("/driverHub");
app.MapControllers();


try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
