using approvals.application;
using approvals.application.DTOs.ApplicationType.Validator;
using approvals.infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using platform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ---- Controllers
builder.Services.AddControllers();

// ---- Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "approvals service",
        Version = "v1"
    });
});

// ---- FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppTypeDtoValidator>();

// ---- App + Infra DI
builder.Services.ConfigurApplicationServices();
builder.Services.AddPersistence(builder.Configuration);

// ---- CORS
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    )
);

var app = builder.Build();

// ---- Apply migrations in Docker
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
    await app.EnsureMigrationAppliedAsync(app.Environment);

// ---- Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API v1")
    );
}

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
