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
    private static readonly string TestEnvironmentName = MyAdditionalEnvironments.HttpIntegrationTest;
    private const string CorsAllowedOriginsForTests = "https://a.b.c;https://d.e.f;https://localhost:3000";

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
            .Enrich.WithProperty(SerilogProperties.EnvironmentName, TestEnvironmentName)
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

        _ = builder.UseEnvironment(TestEnvironmentName);

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

    // Called fourth before application setup
    protected override IHost CreateHost(IHostBuilder builder)
    {
        LogMethodName();

        return base.CreateHost(builder);
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        LogMethodName();

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
        LogMethodName();

        base.ConfigureClient(client);
    }

    private static void LogMethodName([CallerMemberName] string callerName = "")
    {
        Log.Information("Test Fixture: {ClassName}.{MethodName}",nameof(HttpTestFixture), callerName);
    }
}
