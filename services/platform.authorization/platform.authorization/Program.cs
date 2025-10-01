using authorization.api.Configs;
using authorization.application.Abstractions;
using authorization.application.Services;
using authorization.infrastructure.Services;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure OpenFGA with validation
builder.Services.AddOptions<OpenFGAConfig>().Bind(builder.Configuration.GetSection(OpenFGAConfig.SectionName)).ValidateDataAnnotations().ValidateOnStart();

// Register services
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<IOpenFGAService, OpenFGAService>();
builder.Services.AddControllers().AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// OpenFGA Client configuration with error handling
builder.Services.AddSingleton<OpenFgaClient>(provider =>
{
    try
    {
        var config = provider.GetRequiredService<IOptions<OpenFGAConfig>>().Value;
        var logger = provider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Configuring authorizaion provider client with URL: {ApiUrl}", config.BaseUrl);
        logger.LogInformation("Using Store ID: {StoreId}", config.StoreName);

        var clientConfig = new ClientConfiguration()
        {
            ApiUrl = config.BaseUrl,
            StoreId = config.StoreName,
            Credentials = !string.IsNullOrEmpty(config.ApiToken) ? new Credentials
            {
                Method = CredentialsMethod.ApiToken,
                Config = new CredentialsConfig { ApiToken = config.ApiToken }
            } : null
        };

        var client = new OpenFgaClient(clientConfig);

        // Test connection (optional - remove in production if not needed)
        if (builder.Environment.IsDevelopment())
        {
            try
            {
                var stores = client.ListStores().Result;
                logger.LogInformation("Successfully connected to authorizaion provider. Found {StoreCount} stores", stores.Stores?.Count ?? 0);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to connect to OpenFGA during startup test");
            }
        }

        return client;
    }
    catch (Exception ex)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to create OpenFGA client");
        throw;
    }
});

// Add health checks
builder.Services.AddHealthChecks();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Redirect root to Scalar UI
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/scalar");
        return Task.CompletedTask;
    });

    // Log configuration values
    var config = app.Services.GetRequiredService<IOptions<OpenFGAConfig>>().Value;
    app.Logger.LogInformation("OpenFGA Configuration:");
    app.Logger.LogInformation("  ApiUrl: {ApiUrl}", config.BaseUrl);
    app.Logger.LogInformation("  StoreId: {StoreId}", config.StoreName);
    app.Logger.LogInformation("  SchemaVersion: {SchemaVersion}", config.SchemaVersion);
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy")).WithName("HealthCheck").WithOpenApi();

app.Run();