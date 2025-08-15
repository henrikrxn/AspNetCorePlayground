using System.Diagnostics;
using AspNetCorePlayground;
using AspNetCorePlayground.Plumbing;
using AspNetCorePlayground.Plumbing.Configuration;
using AspNetCorePlayground.Plumbing.Setup.Configuration;
using AspNetCorePlayground.Plumbing.Setup.Cors;
using AspNetCorePlayground.Plumbing.Setup.Endpoints;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// This enables ASP.NET Core HTTP integration tests to setup Serilog so that tests gets logging
if (Log.Logger == Serilog.Core.Logger.None)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName() // This only looks at the environment variables, which is not good for e.g. HTTP integration tests
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}")
        .CreateBootstrapLogger();
}
else
{
    Log.Information("Logger already set-up. Skipping Bootstrap logger");
}

WebApplicationBuilder? builder;

try
{
    Log.Information("Creating WebApplication builder");

    builder = WebApplication.CreateBuilder(args);

    Log.Information("Environment: {environmentName}", builder.Environment.EnvironmentName);

    _ = builder.SetupConfigurationValidation();

    if (builder.Environment.IsDevelopment())
    {
        // Serilog internal debug logging
        Log.Information("Setting up Serilog debug logging for development");
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        // Has a performance penalty so  only activating in development
        _ = builder.Host.UseDefaultServiceProvider((_, options) =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        _ = builder.Configuration.AddUserSecrets(typeof(AspNetCorePlayground.Program).Assembly, optional: true, reloadOnChange: true);
    }

    // Logging in general

    // Clearing the pre-registered providers so we know exactly what has been setup
    _ = builder.Logging.ClearProviders();

    // New ASP.NET Core 8 HTTP logging
    _ = builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.CombineLogs = true;
    });

    _ = builder.Host.UseSerilog((hostBuilderContext, serviceProvider, seriLogloggerConfiguration) =>
    {
        _ = seriLogloggerConfiguration
            .ReadFrom.Configuration(hostBuilderContext.Configuration)
            .ReadFrom.Services(serviceProvider)
            .Enrich.WithProperty(SerilogProperties.EnvironmentName, hostBuilderContext.HostingEnvironment.EnvironmentName)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.FromLogContext();

        if (hostBuilderContext.HostingEnvironment.IsProduction())
        {
            Log.Information("Setting up Serilog for production");
            // If console is used in Azure environments there it is probably a good idea to add
            // https://nuget.org/packages/serilog.sinks.async
            // as console historically has been known to slow things down a lot

            // Console is terribly ineffective, so limiting to the really terrible stuff
            _ = seriLogloggerConfiguration.WriteTo.Console(outputTemplate: SerilogTemplates.IncludesProperties,
                restrictedToMinimumLevel: LogEventLevel.Error);
        }
        else
        {
            Log.Information("Setting up Serilog for Environment: '{environmentName}'",
                hostBuilderContext.HostingEnvironment.EnvironmentName);

            _ = seriLogloggerConfiguration.WriteTo.Console(outputTemplate: SerilogTemplates.IncludesProperties);
        }
    }, writeToProviders: !builder.Environment.IsEnvironment(MyAdditionalEnvironments.HttpIntegrationTest));

    // TODO Look into CORS set-up for Minimal API
    _ = builder.Services.AddCors();

    // If I ever want to use "services.AddResponseCaching();" it has to be after CORS

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    _ = builder.Services.AddEndpointsApiExplorer();
    _ = builder.Services.AddOpenApi();

    Log.Information("Building application");
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.Fatal(ex, "Unhandled problems during application setup");
    Log.Information("Flushing and closing Serilog");
    Log.CloseAndFlush();

    return ExitCodes.Error;
}

ILogger loggerAfterBuild = NullLogger.Instance;

try {

    using WebApplication app = builder.Build();

    loggerAfterBuild = app.Logger;
    loggerAfterBuild.UsingEnvironment(builder.Environment.EnvironmentName);
    loggerAfterBuild.AddingMiddleware();

    _ = app.MapOpenApi();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        _ = app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });
    }

    // The normal Microsoft request logging
    _ = app.UseHttpLogging();

    _ = app.UseHttpsRedirection();

    // Before or after CORS ? : app.UseStaticFiles();

    // This must be before CORS : app.UseRouting();

    // CORS TODO Look more into CORS set-up for Minimal API
    _ = app.Cors();

    // This must be after CORS : app.UseAuthorization();

    //
    // Endpoints
    //
    _ = app.MapApplicationEndpoints();

    loggerAfterBuild.StartingApplication();

    app.Run();

    loggerAfterBuild.ApplicationStoppedNormally();

    return ExitCodes.Ok;
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    loggerAfterBuild.CriticalErrorDuringStartup(ex);

    return ExitCodes.Error;
}
finally
{
    loggerAfterBuild.FlushingAndClosingSerilog();

    Log.CloseAndFlush();
}

namespace AspNetCorePlayground
{
    internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

// Expose the Program class so that WebApplicationFactory<T> can access it
// TODO Remove pragmas once .NET 10 becomes RTM
#pragma warning disable CA1052
#pragma warning disable CA1515
    public partial class Program
#pragma warning restore CA1515
#pragma warning restore CA1052
    {
    }

    internal static partial class MyLoggerExtensions
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Environment: {environmentName}")]
        public static partial void UsingEnvironment(this ILogger logger, string environmentName);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Adding middleware")]
        public static partial void AddingMiddleware(this ILogger logger);

        // TODO new EventId
        [LoggerMessage(EventId = 96, Level = LogLevel.Information, Message = "Starting application")]
        public static partial void StartingApplication(this ILogger logger);

        [LoggerMessage(EventId = 97, Level = LogLevel.Critical, Message = "Unhandled problems during application startup")]
        public static partial void CriticalErrorDuringStartup(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 98,  Level = LogLevel.Information, Message = "Application stopped normally")]
        public static partial void ApplicationStoppedNormally(this ILogger logger);

        [LoggerMessage(EventId = 99,  Level = LogLevel.Information, Message = "Flushing and closing Serilog")]
        public static partial void FlushingAndClosingSerilog(this ILogger logger);

        // Logging used in endpoints
        [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = "Generating WeatherForecast")]
        public static partial void GeneratingWeatherForecast(this ILogger logger);

        [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "Throwing exception")]
        public static partial void AboutToThrowException(this ILogger logger);

        [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "Internal Server Error")]
        public static partial void InternalServerError(this ILogger logger);

        [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "Getting configuration")]
        public static partial void GettingConfiguration(this ILogger logger);
    }
}
