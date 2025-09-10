using approvals.application;
using approvals.application.DTOs.ApplicationType.Validator;
using approvals.infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using platform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ---- Controllers
builder.Services.AddControllers();

// ---- Keycloak Authentication (binds from "Keycloak" section by default)
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.RequireHttpsMetadata = false; // dev only
});

// ---- Keycloak Authorization mapping
builder.Services.AddKeycloakAuthorization(options =>
{
    options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
    options.RolesResource = builder.Configuration["Keycloak:resource"];
});

// ---- Authorization Server (client used to call Keycloak Decision/Protection API)
builder.Services.AddAuthorizationServer(options =>
{
    builder.Configuration.BindKeycloakOptions(options);
    options.UseProtectedResourcePolicyProvider = true; // allow "Resource#scope" style policy names
});

// ---- Swagger + JWT UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "approvals service", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---- FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppTypeDtoValidator>();

// ---- App + Infra DI
builder.Services.ConfigurApplicationServices();
builder.Services.AddPersistence(builder.Configuration);

// ---- CORS
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
    await app.EnsureMigrationAppliedAsync(app.Environment);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API v1"));
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
