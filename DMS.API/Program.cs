using DMS.API.Extensions;
using DMS.API.Hubs;
using DMS.API.Middleware;
using DMS.API.Service;
using DMS.Infrastructure;
using DMS.Infrastructure.Data;
using DMS.Infrastructure.Data.Config;
using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ===== RAILWAY DATABASE URL PARSING =====
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        
        var npgsqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        
        builder.Configuration["ConnectionStrings:DefaultConnection"] = npgsqlBuilder.ConnectionString;
        Console.WriteLine("✅ Railway DATABASE_URL parsed successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error parsing DATABASE_URL: {ex.Message}");
    }
}

// Get port from Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Enable legacy timestamp for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// SignalR
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);

// Controllers
builder.Services.AddControllers();

// Register custom services
builder.Services.AddApiRegistration();

// RabbitMQ connection factory
var rabbitHost = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "admin";
var rabbitPass = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "admin";
var rabbitVHost = Environment.GetEnvironmentVariable("RabbitMQ__VHost") ?? "/";

builder.Services.AddSingleton(
    new ConnectionFactory
    {
        HostName = rabbitHost,
        UserName = rabbitUser,
        Password = rabbitPass,
        VirtualHost = rabbitVHost
    });

// Background consumer service
builder.Services.AddHostedService<LogConsumerService>();

// EF Core + PostgreSQL
builder.Services.InfrastructureConfiguration(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Auth Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme,
        }
    };
    s.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "bearer" } } };
    s.AddSecurityRequirement(securityRequirement);
});

// CORS
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", frontendUrl)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Auto-create database and run migrations + SEED DATA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<DataContext>();
    
    try
    {
        // Apply migrations
        db.Database.Migrate();
        Console.WriteLine("✔ Database migrations applied.");
        
        // Seed roles and users
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        
        await IdentitySeed.SeedUserAsync(userManager, roleManager);
        Console.WriteLine("✔ Identity seed completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database setup failed: {ex.Message}");
        throw;
    }
}

app.UseRouting();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");

// CORS
app.UseCors("AllowAngularApp");

// Auth middleware
app.UseAuthentication();
app.UseAuthorization();

// SignalR
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.MapControllers();

// Infrastructure middleware
InfrastructureRegistration.InfrastructureConfigMiddleWare(app);

await app.RunAsync();