using approvals.application;
using approvals.infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using platform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Approvals API",
        Version = "v1",
        Description = "API documentation for the Approvals microservice."
    });
});

// Register class Liberarys 
builder.Services.AddPersistence(builder.Configuration);
builder.Services.ConfigurApplicationServices();

var app = builder.Build();

// Ensure DB migration runs in docker container and development envaroment
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (isRunningInDocker)
    await app.EnsureMigrationAppliedAsync(app.Environment);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
