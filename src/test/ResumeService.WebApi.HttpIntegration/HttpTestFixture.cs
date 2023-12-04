using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace ResumeService.WebApi.HttpIntegration;

public class HttpTestFixture : WebApplicationFactory<Program>, ITestOutputHelperAccessor
{
    public ITestOutputHelper? OutputHelper { get; set; }

    public void ClearOutputHelper() => OutputHelper = null;

    public void SetOutputHelper(ITestOutputHelper value) => OutputHelper = value;

    public HttpTestFixture() // TODO Hvilket environment bruges ? string environment = "HttpTest"
    {
        // Use HTTPS by default
        ClientOptions.BaseAddress = new Uri("https://localhost");
        // Do not follow redirects so they can tested explicitly
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            // TODO DO we want to do this?
            // logging.ClearProviders();
            logging.AddXUnit(this);
            logging.AddConsole();
            /*
            logging.AddSerilog((context, services, configuration) =>
            {

            });
            */
        });

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            // TODO
        });

        // Must be after defaults are configured
        builder.UseEnvironment("KnockKnock");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        return base.CreateHost(builder);
    }
}
