using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Initializers;
using MicroserviceBootstrapper.Interfaces;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddSingleton<Logger>();

        // Bind configuration sections
        services.Configure<AuthenticationConfig>(configuration.GetSection("authentication"));
        services.Configure<AuthorizationConfig>(configuration.GetSection("authorization"));
        services.Configure<ApiGatewayConfig>(configuration.GetSection("apiGateway"));

        // Microservice initializers
        services.AddTransient<IServiceInitializer, AuthenticationInitializer>();
        services.AddTransient<IServiceInitializer, AuthorizationInitializer>();
        services.AddTransient<IServiceInitializer, ApiGatewayInitializer>();

        // OpenFGA Client
        services.AddSingleton<OpenFgaClient>(provider =>
        {
            var authConfig = provider.GetRequiredService<IOptions<AuthorizationConfig>>().Value;

            return new OpenFgaClient(new ClientConfiguration
            {
                ApiUrl = authConfig.BaseUrl,
                Credentials = new Credentials
                {
                    Method = CredentialsMethod.ApiToken,
                    Config = new CredentialsConfig { ApiToken = authConfig.ApiToken }
                }
            });
        });
    })
    .Build();


// ------------------------------------------------------
// EXECUTING INITIALIZERS
// ------------------------------------------------------

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting service initializers...");

var initializers = host.Services.GetServices<IServiceInitializer>();
var hasError = false;

foreach (var initializer in initializers)
{
    try
    {
        logger.LogInformation("Initializing {Initializer}...", initializer.GetType().Name);
        await initializer.InitializeAsync();
        logger.LogInformation("{Initializer} initialized successfully.", initializer.GetType().Name);
    }
    catch (Exception ex)
    {
        hasError = true;
        logger.LogError(ex, "Error initializing {Initializer}", initializer.GetType().Name);
    }
}

if (hasError)
{
    logger.LogError("One or more services failed to initialize. Exiting with failure code.");
    return 1;
}

logger.LogInformation("All services initialized successfully.");
return 0;
