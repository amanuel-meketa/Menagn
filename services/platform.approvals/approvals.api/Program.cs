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

// Add persistence from Infrastructure
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

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
