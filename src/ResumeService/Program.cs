using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    // If ApplicationInsights needs to be added then the configuration will have to be built first
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}")
    // If you want to see the context and added properties, use
    // outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}"
    // as argument for Console() above
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true, reloadOnChange: true);

    // Logging in general

    // Clearing the pre-registered providers so we know exactly what has been setup
    builder.Logging.ClearProviders();

    // New ASP.NET Core 8 HTTP logging
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.CombineLogs = true;
    });

    // Serilog internal debug logging
    if (builder.Environment.IsDevelopment())
    {
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(Console.Error);
    }

    builder.Host.UseSerilog((context, services, configuration) => {

        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.FromLogContext();

        if (context.HostingEnvironment.IsDevelopment())
        {
            configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}");
            // If you want to see the context and added properties, use
            // outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}"
            // as argument for Console() above
        }
        else
        {
            // If console is deemed in Azure environments there it is probably a good idea to add
            // https://nuget.org/packages/serilog.sinks.async
            // as console historically has been known to slow things down a lot
            configuration.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error); // Console is terribly ineffective, so limiting to the really bad stuff
        }
    }, writeToProviders: true);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // The normal Microsoft request logging
    app.UseHttpLogging();

    var summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

    app.Run();

    Log.Information("Application stopped normally");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled problems during application setup and startup");
}
finally
{
    Log.Information("Flushing and closing Serilog");
    Log.CloseAndFlush();
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Expose the Program class so that WebApplicationFactory<T> can access it
public partial class Program { }