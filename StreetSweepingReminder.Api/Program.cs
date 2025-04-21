using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StreetSweepingReminder.Api.DbContext;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreetSweepingReminder.Api.Entities;
using StreetSweepingReminder.Api.Middleware;

[assembly: InternalsVisibleTo("StreetSweepingReminder.Api.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IStreetRepository, StreetRepository>();
builder.Services.AddScoped<IStreetService, StreetService>();
builder.Services.AddScoped<IReminderScheduler, ReminderSchedulerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReminderScheduleRepository, ReminderScheduleRepository>();
builder.Services.AddScoped<IStreetSweepingSchedulerService, StreetSweepingSchedulerService>();
builder.Services.AddScoped<IStreetSweepingDatesRepository, StreetSweepingDatesRepository>();

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://eldo88.github.io", "http://localhost:8080", "http://localhost:5500", "http://localhost:5173", "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
    { //Simple password config, will update in the future
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true; // May remove
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]; // Get from appsettings.json or secrets
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT Key, Issuer or Audience is not configured properly.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
// Get a logger (optional but highly recommended)
var logger = services.GetRequiredService<ILogger<Program>>(); // Use Program or a specific class for logging category

try
{
    logger.LogInformation("Applying database migrations...");

    // Resolve the DbContext
    var context = services.GetRequiredService<ApplicationDbContext>();

    // --- Ensure Database Directory Exists (for SQLite) ---
    var configuration = services.GetRequiredService<IConfiguration>();
    var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
    var dataSourcePrefix = "Data Source=";
    if (!string.IsNullOrEmpty(dbConnectionString) && dbConnectionString.StartsWith(dataSourcePrefix, StringComparison.OrdinalIgnoreCase))
    {
        var filePath = dbConnectionString.Substring(dataSourcePrefix.Length);
        var fullPath = Path.GetFullPath(filePath); // Resolve potential relative paths
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            logger.LogInformation("Database directory not found. Creating: {DirectoryPath}", directory);
            Directory.CreateDirectory(directory);
        }
    }
    // ------------------------------------------------------

    // Apply pending EF Core migrations
    await context.Database.MigrateAsync();

    logger.LogInformation("Database migrations applied successfully.");

    // --- Call Your Data Seeding Logic Here ---
    // Example: Assuming you have a DataSeeder class and CSV file
    // Ensure the seed file is configured with "Copy to Output Directory"
    string seedFilePath = Path.Combine(AppContext.BaseDirectory, "Data/denver_streets.csv");
    logger.LogInformation("Attempting data seeding...");
    await DataSeeder.SeedStreetsAsync(context, seedFilePath);
    logger.LogInformation("Data seeding completed.");
    // -----------------------------------------

}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during database migration or seeding.");
    // IMPORTANT: Decide if the application should stop if migrations fail.
    // Re-throwing the exception here will stop the application startup.
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();