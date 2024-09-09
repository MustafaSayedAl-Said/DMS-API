using DMS.API.Extensions;
using DMS.API.Hubs;
using DMS.API.Middleware;
using DMS.API.Service;
using DMS.Infrastructure;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; });
builder.Services.AddControllers();
builder.Services.AddApiRegistration();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHostedService<LogConsumerService>();

builder.Services.AddSingleton(
           new ConnectionFactory
           {
               HostName = "localhost",
               UserName = "user",
               Password = "mypass",
               VirtualHost = "/"
           });

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
builder.Services.InfrastructureConfiguration(builder.Configuration);




var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();


app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

// Enable CORS for the specified policy
app.UseCors("AllowAngularApp");

app.UseAuthentication();

app.UseAuthorization();

//app.MapHub<NotificationHub>("/notificationHub");
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.MapControllers();

InfrastructureRegistration.InfrastructureConfigMiddleWare(app);

await app.RunAsync();
