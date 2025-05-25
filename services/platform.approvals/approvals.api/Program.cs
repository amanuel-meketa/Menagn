using approvals.application;
using approvals.infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using platform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();

// Add services from all class liberary
builder.Services.AddPersistence(builder.Configuration);
builder.Services.ConfigurApplicationServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API v1");
        options.RoutePrefix = "swagger"; // Serves Swagger UI at /swagger
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
