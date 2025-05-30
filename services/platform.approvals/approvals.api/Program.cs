using approvals.api.Middleware;
using approvals.application;
using approvals.application.DTOs.ApplicationType.Validator;
using approvals.infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using platform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppTypeDtoValidator>();

// Application and Infrastructure DI
builder.Services.ConfigurApplicationServices();
builder.Services.AddPersistence(builder.Configuration);

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Use global exception handler middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Run migrations if inside Docker
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
    await app.EnsureMigrationAppliedAsync(app.Environment);

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API v1");
        options.RoutePrefix = "swagger";
    });
}

// Standard middleware order
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();