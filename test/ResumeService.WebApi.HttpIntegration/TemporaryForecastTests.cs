using Xunit.Abstractions;

namespace ResumeService.Test.WebApi.HttpIntegration;

[Collection(ResumeAppCollection.Name)]
public class ApiTests
{
    public ApiTests(HttpTestFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;

        // Route output from the fixture's logs to xunit's output
        OutputHelper = outputHelper;
        Fixture.SetOutputHelper(OutputHelper);
    }

    private HttpTestFixture Fixture { get; }

    private ITestOutputHelper OutputHelper { get; }

    [Fact]
    public async Task WhenCallingWeatherForecast_ThenWeAlwaysGetFiveForecasts()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        IWebHostEnvironment webHostEnvironment = Fixture.Services.GetRequiredService<IWebHostEnvironment>();
        OutputHelper.WriteLine("Test code: Environment={0}", webHostEnvironment.EnvironmentName);

        // Act
        WeatherForecast[]? forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        _ = forecasts.Should().NotBeNull();
        _ = forecasts.Should().HaveCount(5);
    }

    // Duplicating instead of referencing the one in the ResumeService in order to test the "contract" not the existing implementation
    internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
}
