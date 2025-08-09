using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using AspNetCorePlayground;
using AspNetCorePlayground.Plumbing;
using AspNetCorePlayground.Plumbing.Configuration;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

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

try
{
    Log.Information("Creating WebApplication builder");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);

    builder.Services.AddOptionsWithValidateOnStart<DictionaryConfiguration>()
        .Bind(builder.Configuration.GetSection(DictionaryConfiguration.SectionName))
        .ValidateDataAnnotations();

    // Has a performance penalty so could consider only activating in development
    builder.Host.UseDefaultServiceProvider((_, options) =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

    if (builder.Environment.IsDevelopment())
    {
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

    // Serilog internal debug logging
    if (builder.Environment.IsDevelopment())
    {
        Log.Information("Setting up Serilog debug logging for development");
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(Console.Error);
    }

    _ = builder.Host.UseSerilog((context, services, configuration) =>
    {
        _ = configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithProperty(SerilogProperties.EnvironmentName, context.HostingEnvironment.EnvironmentName)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.FromLogContext();

        if (context.HostingEnvironment.IsProduction())
        {
            Log.Information("Setting up Serilog for production");
            // If console is used in Azure environments there it is probably a good idea to add
            // https://nuget.org/packages/serilog.sinks.async
            // as console historically has been known to slow things down a lot

            // Console is terribly ineffective, so limiting to the really terrible stuff
            _ = configuration.WriteTo.Console(outputTemplate: SerilogTemplates.IncludesProperties, restrictedToMinimumLevel: LogEventLevel.Error);
        }
        else
        {
            Log.Information("Setting up Serilog for Environment: '{Environment}'", context.HostingEnvironment.EnvironmentName);

            _ = configuration.WriteTo.Console(outputTemplate: SerilogTemplates.IncludesProperties);
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

    using WebApplication app = builder.Build();

    Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);

    Log.Information("Adding middleware");

    app.MapOpenApi();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });
    }

    // The normal Microsoft request logging
    _ = app.UseHttpLogging();

    _ = app.UseHttpsRedirection();

    // Before or after CORS ? : app.UseStaticFiles();

    // This must be before CORS : app.UseRouting();

    // TODO Look into CORS set-up for Minimal API
    app.UseCors(corsPolicyBuilder =>
    {
        // TODO Can this be moved back up in 'ConfigureServices' after .NET 6 wih the new cool way of having "things" ready earlier ?
        corsPolicyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod(); // TODO look into narrowing this

        var allowedOriginsConfigSection =
            builder.Configuration.GetSection(ConfigurationPaths.CorsAllowedOrigins);
        var semicolonSeparatedOrigins = allowedOriginsConfigSection.Get<string>() ??
                                        throw new ValidationException($"Configuration path '{ConfigurationPaths.CorsAllowedOrigins}' must contain at least one CORS allowed origin");
        var allowedOrigins =
            semicolonSeparatedOrigins.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // TODO Move all this to a separate configuration object
        if (allowedOrigins.Length > 0)
        {
            if (allowedOrigins.Any(origin => "*".Equals(origin)))
            {
                corsPolicyBuilder.AllowAnyOrigin();
            }
            else
            {
                foreach (var origin in allowedOrigins)
                {
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out _))
                    {
                        throw new ValidationException(
                            $"Origin '{origin}' in configuration path '{ConfigurationPaths.CorsAllowedOrigins}' cannot be parsed as an Uri");
                    }
                }

                corsPolicyBuilder.WithOrigins(allowedOrigins);
            }
        }
        else
        {
            throw new ValidationException($"Configuration path '{ConfigurationPaths.CorsAllowedOrigins}' must contain at least one CORS allowed origin");
        }
    });

    // This must be after CORS : app.UseAuthorization();

    string[] summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];
    _ = app.MapGet("/weatherforecast", () =>
        {
            WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        RandomNumberGenerator.GetInt32(-20, 55),
                        summaries[RandomNumberGenerator.GetInt32(0, summaries.Length)]
                    ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

    _ = app.MapGet("/throws", () =>
    {
        throw new WebException("My exception");
    });

    _ = app.MapGet("/internalServerError", () => TypedResults.InternalServerError("Something went wrong!"));

    _ = app.MapGet("/config/dictionary", (IOptions<DictionaryConfiguration> dictionaryOptions) =>
    {
        DictionaryConfiguration dictionary = dictionaryOptions.Value;

        return dictionary.Items is { Count: > 0 } ? Results.Ok(dictionary.Items) : Results.NotFound("Configuration items not found.");
    });

    Log.Information("Starting application");

    app.Run();

    Log.Information("Application stopped normally");
    return ExitCodes.Ok;
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.Fatal(ex, "Unhandled problems during application setup and startup");
    return ExitCodes.Error;
}
finally
{
    Log.Information("Flushing and closing Serilog");
    Log.CloseAndFlush();
}

namespace AspNetCorePlayground
{
    internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

// Expose the Program class so that WebApplicationFactory<T> can access it
// TODO Remove once .NET 10 becomes RTM
#pragma warning disable CA1052
#pragma warning disable CA1515
    public partial class Program
#pragma warning restore CA1515
#pragma warning restore CA1052
    {
        // TODO Move this to separate config class

    }
}
