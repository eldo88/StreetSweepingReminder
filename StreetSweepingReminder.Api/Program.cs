using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using DbUp;
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

var environmentName = builder.Environment.EnvironmentName;

// DbUp
if (builder.Environment.IsProduction())
{
    try
    {
        Console.WriteLine("Starting DbUp migrations...");
        var upgradeEngine = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();
   
        var result = upgradeEngine.PerformUpgrade();
   
        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DbUp migration failed:");
            Console.WriteLine(result.Error);
            Console.ResetColor();
            throw new Exception("DbUp database migration failed.", result.Error);
        }
   
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("DbUp migration successful!");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("An error occurred during DbUp execution:");
        Console.WriteLine(ex);
        Console.ResetColor();
        throw;
    }
} else
{
    Console.WriteLine("Skipping DbUp migrations (Not Production Environment or Design Time).");
}


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
