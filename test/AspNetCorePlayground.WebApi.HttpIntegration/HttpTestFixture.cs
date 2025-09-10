using System.Globalization;
using System.Runtime.CompilerServices;
using AspNetCorePlayground.Plumbing;
using AspNetCorePlayground.Plumbing.Configuration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace AspNetCorePlayground.WebApi.HttpIntegration;

public class HttpTestFixture : WebApplicationFactory<Program>
{
    private static readonly string _testEnvironmentName = MyAdditionalEnvironments.HttpIntegrationTest;
    private readonly List<string> _corsAllowedOriginsForTests = [ "https://a.b.c", "https://d.e.f", "https://localhost:3000" ];

    public HttpTestFixture()
    {
        // Use HTTPS by default
        ClientOptions.BaseAddress = new Uri("https://localhost");
        // Do not follow redirects so that redirects can be tested explicitly
        ClientOptions.AllowAutoRedirect = false;
    }

    // Called first before application setup
    protected override IHostBuilder? CreateHostBuilder()
    {
        // Set-up Serilog to log to console. This BootstrapLogger is replaced below after startup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty(SerilogProperties.EnvironmentName, _testEnvironmentName)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();

        LogMethodName();

        return base.CreateHostBuilder();
    }

    // Called second before application setup
    protected override IWebHostBuilder? CreateWebHostBuilder()
    {
        LogMethodName();

        return base.CreateWebHostBuilder();
    }

    // Called third before application setup
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        LogMethodName();

        _ = builder.UseEnvironment(_testEnvironmentName);

        // Executed after set-up code in Program
        _ = builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            IEnumerable<KeyValuePair<string, string?>> inMemoryCorsValues =
                _corsAllowedOriginsForTests.Select((value, index) => KeyValuePair.Create<string, string?>($"{CorsConfiguration.CorsAllowedOrigins}:{index}", value));
            _ = configurationBuilder.AddInMemoryCollection(inMemoryCorsValues);

            LogMessage($"Inside {nameof(builder.ConfigureAppConfiguration)}");
        });

        var corsSettings = builder.GetSetting(CorsConfiguration.CorsAllowedOrigins);

        // Add mock/test services to the builder here
        _ = builder.ConfigureServices((webHostBuilderContext, services) => { });
    }

    // Called fourth before application setup
    protected override IHost CreateHost(IHostBuilder builder)
    {
        LogMethodName();

        // Executed after set-up code in Program
        _ = builder.ConfigureLogging(loggingBuilder =>
        {
            LogMessage("Inside CreateHost.ConfigureLogging");
            // Clear all existing providers (like Console, Debug)
            _ = loggingBuilder.ClearProviders();

            // Configure Serilog to work with the ILoggerFactory
            _ = loggingBuilder.AddSerilog(
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                    .CreateLogger());
        });

        return base.CreateHost(builder);
    }

    // Called first after application set-up
    protected override void ConfigureClient(HttpClient client)
    {
        LogMethodName();

        base.ConfigureClient(client);
    }

    private static void LogMethodName([CallerMemberName] string callerName = "")
    {
        Log.Information("Test Fixture: {ClassName}.{MethodName}",nameof(HttpTestFixture), callerName);
    }

    private static void LogMessage(string message, [CallerMemberName] string callerName = "")
    {
        Log.Information("Test Fixture: {ClassName}.{MethodName} - {Message}", nameof(HttpTestFixture), callerName, message);
    }
}
