using System.Net;
using AspNetCorePlayground.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class ValidationTests
{
    public ValidationTests(HttpTestFixture fixture)
    {
        Fixture = fixture;
    }

    private HttpTestFixture Fixture { get; }
    private static readonly string[] LatitudeError = [$"The field {nameof(GeodeticEarthSurface.Latitude)} must be between -90 and 90."];
    private static readonly string[] LongitudeError = [$"The field {nameof(GeodeticEarthSurface.Longitude)} must be between -180 and 180."];

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(90.0, 180.0)]
    [InlineData(-90.0, 180.0)]
    [InlineData(90.0, -180.0)]
    [InlineData(-90.0, -180.0)]
    public async Task WhenInLegalRange_ThenReturnsOk(decimal latitude, decimal longitude)
    {
        HttpClient client = Fixture.CreateClient();
        var body = new { Latitude = latitude, Longitude = longitude };
        using var request = new HttpRequestMessage(HttpMethod.Post, "/validation");
        request.Content = JsonContent.Create(body);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _ = response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(90.1)]
    [InlineData(-90.1)]
    public async Task WhenLatitudeNotInLegalRange_ThenReturnsBadRequestWithValidationError(decimal latitude)
    {
        HttpClient client = Fixture.CreateClient();
        var body = new { Latitude = latitude, Longitude = 0.0 };
        using var request = new HttpRequestMessage(HttpMethod.Post, "/validation");
        request.Content = JsonContent.Create(body);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var validationErrors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        validationErrors.ShouldNotBeNull();
        validationErrors.Errors.Count.ShouldBe(1);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurface.Latitude));
        validationErrors.Errors[nameof(GeodeticEarthSurface.Latitude)].ShouldBeEquivalentTo(LatitudeError);
    }

    [Theory]
    [InlineData(180.1)]
    [InlineData(-180.1)]
    public async Task WhenLongitudeNotInLegalRange_ThenReturnsBadRequestWithValidationError(decimal longitude)
    {
        HttpClient client = Fixture.CreateClient();
        var body = new { Latitude = 0.0, Longitude = longitude };
        using var request = new HttpRequestMessage(HttpMethod.Post, "/validation");
        request.Content = JsonContent.Create(body);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var validationErrors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        validationErrors.ShouldNotBeNull();
        validationErrors.Errors.Count.ShouldBe(1);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurface.Longitude));
        validationErrors.Errors[nameof(GeodeticEarthSurface.Longitude)].ShouldBeEquivalentTo(LongitudeError);
    }

    [Fact]
    public async Task WhenLatitudeAndLongitudeNotInLegalRange_ThenReturnsBadRequestWithValidationErrors()
    {
        HttpClient client = Fixture.CreateClient();
        var body = new { Latitude = 90.1, Longitude = 180.1 };
        using var request = new HttpRequestMessage(HttpMethod.Post, "/validation");
        request.Content = JsonContent.Create(body);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var validationErrors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        validationErrors.ShouldNotBeNull();
        validationErrors.Errors.Count.ShouldBe(2);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurface.Latitude));
        validationErrors.Errors[nameof(GeodeticEarthSurface.Latitude)].ShouldBeEquivalentTo(LatitudeError);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurface.Longitude));
        validationErrors.Errors[nameof(GeodeticEarthSurface.Longitude)].ShouldBeEquivalentTo(LongitudeError);
    }
}
