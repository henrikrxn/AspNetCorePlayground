using AspNetCorePlayground.Plumbing;
using AspNetCorePlayground.Plumbing.Configuration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
// TODO using Serilog.Sinks.XUnit.Injectable;
// TODO using Serilog.Sinks.XUnit.Injectable.Extensions;

namespace AspNetCorePlayground.WebApi.HttpIntegration;

// TODO Cleanup debug messages in class as it will be noise later

public class HttpTestFixture : WebApplicationFactory<Program>
{
    public ITestOutputHelper? OutputHelper { get; set; }

    protected static readonly string TestEnvironmentName = MyAdditionalEnvironments.HttpIntegrationTest;
    public const string CorsAllowedOriginsForTests = "https://a.b.c;https://d.e.f;https://localhost:3000";

    private ITestOutputHelper OutputHelperSet => OutputHelper ?? throw new NullValueMissingInitializeException(nameof(OutputHelper));

    public void ClearOutputHelper() => OutputHelper = null;

    public void SetOutputHelper(ITestOutputHelper value) => OutputHelper = value;

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
        OutputHelperSet.WriteLine($"Test code: {nameof(ConfigureWebHost)}");

        _ = builder.UseEnvironment(TestEnvironmentName);

        // Set-up Serilog to use XUnit TestOutputHelper. Not injecting this instance because Log.Logger is a bootstrap logger and will be
        // overwritten during start-up
        // TODO InjectableTestOutputSink injectableTestOutputSink = new(outputTemplate: SerilogTemplates.IncludesProperties);
        // TODO injectableTestOutputSink.Inject(OutputHelperSet);
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(SerilogProperties.EnvironmentName, TestEnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                // TODO .WriteTo.InjectableTestOutput(injectableTestOutputSink)
                .CreateBootstrapLogger();

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            IList<KeyValuePair<string, string?>> inMemoryCorsValues =
            [
                KeyValuePair.Create<string, string?>(ConfigurationPaths.CorsAllowedOrigins, CorsAllowedOriginsForTests)
            ];
            configurationBuilder
                .AddInMemoryCollection(inMemoryCorsValues);
        });

        // Add mock/test services to the builder here
        _ = builder.ConfigureServices((webHostBuilderContext, services) =>
        {
            // Registering the Serilog sink for XUnit in services to that the Serilog configuration in Program picks it up automagically
            /* TODO
            _ = services.AddSingleton<ILogEventSink, InjectableTestOutputSink>(sp =>
            {
                InjectableTestOutputSink injectableTestOutputSinkInClosure = new(outputTemplate: SerilogTemplates.IncludesProperties);
                injectableTestOutputSinkInClosure.Inject(OutputHelperSet);
                return injectableTestOutputSinkInClosure;
            });
            */
        });
    }

    protected override IHostBuilder? CreateHostBuilder()
    {
        OutputHelperSet.WriteLine($"Test code: {nameof(CreateHostBuilder)}");
        return base.CreateHostBuilder();
    }

    protected override IWebHostBuilder? CreateWebHostBuilder()
    {
        OutputHelperSet.WriteLine($"Test code: {nameof(CreateWebHostBuilder)}");
        return base.CreateWebHostBuilder();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        OutputHelperSet.WriteLine($"Test code: {nameof(CreateHost)}");
        return base.CreateHost(builder);
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        OutputHelperSet.WriteLine($"Test code: {nameof(CreateServer)}");
        return base.CreateServer(builder);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        OutputHelperSet.WriteLine($"Test code: {nameof(ConfigureClient)}");
        base.ConfigureClient(client);
    }
}
