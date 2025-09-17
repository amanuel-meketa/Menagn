﻿using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Initializers;
using MicroserviceBootstrapper.Interfaces;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(config =>{ config.AddConsole(); config.SetMinimumLevel(LogLevel.Information); });
        services.AddSingleton<Logger>();

        var configuration = hostContext.Configuration;

        // Bind configurations for all supported services
        services.Configure<AuthenticationConfig>(configuration.GetSection("authentication"));
        services.Configure<AuthorizationConfig>(configuration.GetSection("authorization"));

        // Register all initializers here
        services.AddTransient<IServiceInitializer, AuthenticationInitializer>();
        services.AddTransient<IServiceInitializer, AuthorizationInitializer>();
    })
    .Build();

// Resolve logger
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting service initializers...");

// Resolve all initializers
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
    return 1; // Exit code for failure
}

logger.LogInformation("All services initialized successfully.");
return 0; // Exit code for success
