namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class ApiTests
{
    public ApiTests(HttpTestFixture fixture)
    {
        Fixture = fixture;
    }

    private HttpTestFixture Fixture { get; }

    [Fact]
    public async Task WhenCallingWeatherForecast_ThenWeAlwaysGetFiveForecasts()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        IWebHostEnvironment webHostEnvironment = Fixture.Services.GetRequiredService<IWebHostEnvironment>();
        Console.WriteLine("Test code: Environment={0}", webHostEnvironment.EnvironmentName);

        // Act
        WeatherForecast[]? forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast", TestContext.Current.CancellationToken);

        // Assert
        _ = forecasts.ShouldNotBeNull();
        forecasts.Length.ShouldBe(5);
    }

    // Duplicating instead of referencing the one in the ResumeService in order to test the "contract" not the existing implementation
#pragma warning disable CA1812
    internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
#pragma warning restore CA1812
}
