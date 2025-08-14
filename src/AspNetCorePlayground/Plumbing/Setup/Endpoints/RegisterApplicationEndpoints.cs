using System.Net;
using System.Security.Cryptography;
using AspNetCorePlayground.Plumbing.Configuration;
using Microsoft.Extensions.Options;

namespace AspNetCorePlayground.Plumbing.Endpoints;

public static class RegisterApplicationEndpoints
{
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        _ = app.MapGet("/weatherforecast", static (ILogger<Program> logger) =>
            {
                logger.GeneratingWeatherForecast();

                string[] summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];
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

        _ = app.MapGet("/throws", static (ILogger<Program> logger) =>
        {
            logger.AboutToThrowException();

            throw new WebException("My exception");
        });

        _ = app.MapGet("/internalServerError", static (ILogger<Program> logger) =>
        {
            logger.InternalServerError();

            return TypedResults.InternalServerError("Something went wrong!");
        });

        _ = app.MapGet("/config/dictionary", (ILogger<Program> logger, IOptions<DictionaryConfiguration> dictionaryOptions) =>
        {
            logger.GettingConfiguration();

            DictionaryConfiguration dictionary = dictionaryOptions.Value;

            return dictionary.Items is { Count: > 0 } ? Results.Ok(dictionary.Items) : Results.NotFound("Configuration items not found.");
        });


        return app;
    }
}
