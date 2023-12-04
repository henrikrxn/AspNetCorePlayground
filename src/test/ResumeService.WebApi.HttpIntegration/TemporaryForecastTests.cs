using FluentAssertions;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ResumeService.WebApi.HttpIntegration;

public class ApiTests
{
    [Fact]
    public async Task WhenCallingWeatherForecast_ThenWeAlwaysGetFiveForecasts()
    {
        // Arrange
        await using var fixture = new HttpTestFixture();
        using var client = fixture.CreateClient();

        // Act
        var forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        forecasts.Should().NotBeNull();
        forecasts.Should().HaveCount(5);

    }

    // Duplicating instead of referencing the one in the ResumeService in order to test the "contract" not the existing implementation
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
}
