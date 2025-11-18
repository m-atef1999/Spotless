using Spotless.API.Extensions;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Configurations;
using Spotless.Infrastructure.SeedData;
using Spotless.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddRepositories()
    .AddIdentityAndAuthentication(builder.Configuration)
    .AddApplicationServices();

// Configure settings using IOptions pattern
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

builder.Services.AddSingleton<IPaymentGatewayService, PaymentGatewayService>();
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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();