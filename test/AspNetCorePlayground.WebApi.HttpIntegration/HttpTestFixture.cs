using System.Globalization;
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
    private static readonly string TestEnvironmentName = MyAdditionalEnvironments.HttpIntegrationTest;
    private const string CorsAllowedOriginsForTests = "https://a.b.c;https://d.e.f;https://localhost:3000";

    public HttpTestFixture()
    {
        // Use HTTPS by default
        ClientOptions.BaseAddress = new Uri("https://localhost");
        // Do not follow redirects so that redirects can be tested explicitly
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // ConfigureWebHost is executed before any code in the application code, e.g. Program
        // The builder.ConfigureXYZ methods, e.g. ConfigureAppConfiguration, are executed AFTER the same methods in Program,
        Console.WriteLine($"Test code: {nameof(ConfigureWebHost)}");

        _ = builder.UseEnvironment(TestEnvironmentName);

        // Set-up Serilog to use XUnit TestOutputHelper. Not injecting this instance because Log.Logger is a bootstrap logger and will be
        // overwritten during start-up
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(SerilogProperties.EnvironmentName, TestEnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .CreateBootstrapLogger();

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            IList<KeyValuePair<string, string?>> inMemoryCorsValues =
            [
                KeyValuePair.Create<string, string?>(ConfigurationPaths.CorsAllowedOrigins, CorsAllowedOriginsForTests)
            ];
            configurationBuilder.AddInMemoryCollection(inMemoryCorsValues);
        });

        // Add mock/test services to the builder here
        _ = builder.ConfigureServices((webHostBuilderContext, services) => { });
    }

    protected override IHostBuilder? CreateHostBuilder()
    {
        Console.WriteLine($"Test code: {nameof(CreateHostBuilder)}");
        return base.CreateHostBuilder();
    }

    protected override IWebHostBuilder? CreateWebHostBuilder()
    {

        Console.WriteLine($"Test code: {nameof(CreateWebHostBuilder)}");
        return base.CreateWebHostBuilder();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Console.WriteLine($"Test code: {nameof(CreateHost)}");
        return base.CreateHost(builder);
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        Console.WriteLine($"Test code: {nameof(CreateServer)}");

        builder.ConfigureLogging(loggingBuilder =>
        {
            // Clear all existing providers (like Console, Debug)
            loggingBuilder.ClearProviders();

            // Configure Serilog to work with the ILoggerFactory
            loggingBuilder.AddSerilog(
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                    .CreateLogger()
            );
        });

        return base.CreateServer(builder);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        Console.WriteLine($"Test code: {nameof(ConfigureClient)}");
        base.ConfigureClient(client);
    }
}
