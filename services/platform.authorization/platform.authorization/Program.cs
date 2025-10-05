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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Bind OpenFGA config with environment variable support
builder.Configuration.AddEnvironmentVariables(); // Important for Docker env vars
builder.Services.AddOptions<OpenFGAConfig>().Bind(builder.Configuration.GetSection(OpenFGAConfig.SectionName)).ValidateDataAnnotations().ValidateOnStart();

// Register services
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<IOpenFGAService, OpenFGAService>();
builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Configure OpenFGA Client
builder.Services.AddSingleton<OpenFgaClient>(provider =>
{
    var config = provider.GetRequiredService<IOptions<OpenFGAConfig>>().Value;
    var logger = provider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Configuring Authorization Provider client");
    logger.LogInformation("Store ID: {StoreId}", config.StoreName);

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

    // Optional: test connection in development
    if (builder.Environment.IsDevelopment())
    {
        try
        {
            var stores = client.ListStores().Result;
            logger.LogInformation("Connected to Authorization Provider. Stores found: {StoreCount}", stores.Stores?.Count ?? 0);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to connect to Authorization Provider during startup test");
        }
    }

    return client;
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/scalar");
        return Task.CompletedTask;
    });

    // Log effective configuration
    var config = app.Services.GetRequiredService<IOptions<OpenFGAConfig>>().Value;
    app.Logger.LogInformation("Effective AuthorizationProvider Config:");
    app.Logger.LogInformation("  BaseUrl: {BaseUrl}", config.BaseUrl);
    app.Logger.LogInformation("  StoreId: {StoreId}", config.StoreName);
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy")).WithOpenApi();

app.Run();