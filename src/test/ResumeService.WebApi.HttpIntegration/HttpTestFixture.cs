using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        //     private readonly string _environment;
        // _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        // builder.UseEnvironment(_environment);

        builder.ConfigureLogging(logging =>
         {
             logging.ClearProviders();
             logging.AddXUnit(this);
         });        

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {

        });

        return base.CreateHost(builder);
    }
}
