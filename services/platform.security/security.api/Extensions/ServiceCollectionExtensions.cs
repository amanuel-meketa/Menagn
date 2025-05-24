using Microsoft.OpenApi.Models;

namespace Keycloak.Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(o =>
        {
            // Use full class name to avoid schema ID conflicts
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var authorizationUrl = configuration["Authentication:AuthorizationUrl"];
            var tokenUrl = configuration["Authentication:TokenUrl"];

            // Define OAuth2 scheme for Keycloak
            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Description = "OAuth2 AuthorizationCode Flow using Keycloak",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationUrl!),
                        TokenUrl = new Uri(tokenUrl!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "Access OpenID scope" },
                            { "profile", "Access profile info" }
                        }
                    }
                }
            });

            // Add security requirement referencing the above scheme
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Keycloak"
                        }
                    },
                    new List<string> { "openid", "profile" }
                }
            });
        });

        return services;
    }
}
