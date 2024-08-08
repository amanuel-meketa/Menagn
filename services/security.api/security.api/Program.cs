using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;
using Microsoft.OpenApi.Models;
using security.business.Contracts;
using security.business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var keycloakOptions = new KeycloakAdminClientOptions
{
    AuthServerUrl = builder.Configuration["Keycloak:AuthServerUrl"],
    Realm = builder.Configuration["Keycloak:Realm"],
    Resource = builder.Configuration["Keycloak:Resource"]
};

// Add Keycloak Admin HTTP client
builder.Services.AddKeycloakAdminHttpClient(builder.Configuration)
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(keycloakOptions.AuthServerUrl);
    });

builder.Services.AddControllers();
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IdentityService>();

/// <summary>
/// Keycloak roles can be automatically transformed to AspNetCore Roles. This feature is disabled by
/// default and is based on KeycloakRolesClaimsTransformation
/// </summary>
builder.Services.AddAuthorization()
    .AddKeycloakAuthorization(options =>
    {
        options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
        options.RolesResource = builder.Configuration["Keycloak:resource"];
    })
    .AddAuthorizationBuilder();

AddSwaggerDoc(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Menagn Software Api v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

void AddSwaggerDoc(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Imagine Task Api",
        });
    });
}
