using FluentAssertions;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ResumeService.WebApi.HttpIntegration;

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
        using var client = Fixture.CreateClient();

        // Act
        var forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        forecasts.Should().NotBeNull();
        forecasts.Should().HaveCount(5);
    }

    // Duplicating instead of referencing the one in the ResumeService in order to test the "contract" not the existing implementation
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
}
