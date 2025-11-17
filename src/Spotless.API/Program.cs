using Spotless.API.Extensions;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Configurations;
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
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SettingsKey));
builder.Services.Configure<PaymentGatewaySettings>(
    builder.Configuration.GetSection(PaymentGatewaySettings.SettingsKey));
builder.Services.AddSingleton<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.Configure<ReviewSettings>(
    builder.Configuration.GetSection(ReviewSettings.SectionName));
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Attempting to seed database...");

try
{
    await Spotless.Infrastructure.Data.DbInitializer.SeedAsync(app.Services);
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