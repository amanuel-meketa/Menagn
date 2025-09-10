using Keycloak.Auth.Api.Extensions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using security.business.Contracts;
using security.business.Services;
using security.sharedUtils;

var builder = WebApplication.CreateBuilder(args);

var authConfig = builder.Configuration.GetSection("Authentication");

builder.Services.AddControllers();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);
builder.Services.ConfigurApplicationServices();
builder.Services.AddKeycloakWebApiAuthentication(
builder.Configuration,
    options =>
    {
        options.Authority = authConfig["Authority"];
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidIssuers = new[] { authConfig["Authority"] };
        options.RequireHttpsMetadata = false;
    }
);

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEventLogService, EventLogService>();
builder.Services.AddHttpClient<IdentityService>();

/// <summary>
/// Keycloak roles can be automatically transformed to AspNetCore Roles. This feature is disabled by
/// default and is based on KeycloakRolesClaimsTransformation
/// </summary>
builder.Services.AddAuthorization().AddKeycloakAuthorization(options =>{
        options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
        options.RolesResource = builder.Configuration["Keycloak:resource"];
    }).AddAuthorizationBuilder();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() .AllowAnyHeader().AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Security Service Api v1");
        var clientId = builder.Configuration["Keycloak:resource"];
        var clientSecret = builder.Configuration["Keycloak:credentials:secret"];

        c.OAuthClientId(clientId);                     // Pre-fill client_id
        c.OAuthClientSecret(clientSecret);             // Pre-fill client_secret (may not autofill due to browser security)
        c.OAuthUsePkce();                              // For Authorization Code Flow
        c.OAuthAppName("Menagn Swagger UI");
        c.OAuthScopes("openid", "profile");            // Pre-selected scopes
    });
}

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
