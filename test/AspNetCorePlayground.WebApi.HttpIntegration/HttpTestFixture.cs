using AspNetCorePlayground;
using AspNetCorePlayground.Plumbing;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.TestHost;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.XUnit.Injectable;
using Serilog.Sinks.XUnit.Injectable.Extensions;
using Xunit.Abstractions;

namespace ResumeService.Test.WebApi.HttpIntegration;

// TODO Cleanup debug messages in class as it will be noise later

public class HttpTestFixture : WebApplicationFactory<Program>, ITestOutputHelperAccessor // TODO Make my own interface and get rid of dependency
{
    public ITestOutputHelper? OutputHelper { get; set; }

#pragma warning disable IDE1006 // Naming Styles
    private readonly string TestEnvironmentName = MyAdditionalEnvironments.HttpIntegrationTest;
#pragma warning restore IDE1006 // Naming Styles

    private ITestOutputHelper OutputHelperSet => OutputHelper ?? throw new NullValueMissingInitializeException(nameof(OutputHelper));

    public void ClearOutputHelper() => OutputHelper = null;

    public void SetOutputHelper(ITestOutputHelper value) => OutputHelper = value;

    public HttpTestFixture()
    {
        // Use HTTPS by default
        ClientOptions.BaseAddress = new Uri("https://localhost");
        // Do not follow redirects so they can tested explicitly
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // This is executed before any code in the application code, e.g. Program
        // But that seems wrong to me shouldn't this be AFTER the configuration code in Program,
        // but before the application is built in Program ?
        OutputHelperSet.WriteLine($"Test code: {nameof(ConfigureWebHost)}");

        _ = builder.UseEnvironment(TestEnvironmentName);

        // Set-up Serilog to use XUnit TestOutputHelper. Not injecting this instance because Log.Logger is a bootstrap logger and will be
        // overwritten during start-up
        InjectableTestOutputSink injectableTestOutputSink = new(outputTemplate: SerilogTemplates.IncludesProperties);
        injectableTestOutputSink.Inject(OutputHelperSet);
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(SerilogProperties.EnvironmentName, TestEnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .WriteTo.InjectableTestOutput(injectableTestOutputSink)
                .CreateBootstrapLogger();

        // Add mock/test services to the builder here
        _ = builder.ConfigureServices((webHostBuilderContext, services) =>
        {
            // Registering the Serilog sink for XUnit in services to that the Serilog configuration in Program picks it up automagically
            _ = services.AddSingleton<ILogEventSink, InjectableTestOutputSink>(sp =>
            {
                InjectableTestOutputSink injectableTestOutputSinkInClosure = new(outputTemplate: SerilogTemplates.IncludesProperties);
                injectableTestOutputSinkInClosure.Inject(OutputHelperSet);
                return injectableTestOutputSinkInClosure;
            });
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
