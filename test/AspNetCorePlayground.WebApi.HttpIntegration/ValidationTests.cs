using System.Net;
using AspNetCorePlayground.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class ValidationTests
{
    public ValidationTests(HttpTestFixture fixture)
    {
        Fixture = fixture;
    }

    private HttpTestFixture Fixture { get; }
    private readonly string[] LatitudeRangeError = [$"The field {nameof(GeodeticEarthSurfaceDto.Latitude)} must be between -90 and 90."];
    private readonly string[] LongitudeRangeError = [$"The field {nameof(GeodeticEarthSurfaceDto.Longitude)} must be between -180 and 180."];

    private readonly string[] LatitudeRequiredError = [$"The {nameof(GeodeticEarthSurfaceDto.Latitude)} field is required."];
    private readonly string[] LongitudeRequiredError = [$"The {nameof(GeodeticEarthSurfaceDto.Longitude)} field is required."];

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
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Latitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Latitude)].ShouldBeEquivalentTo(LatitudeRangeError);
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
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Longitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Longitude)].ShouldBeEquivalentTo(LongitudeRangeError);
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
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Latitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Latitude)].ShouldBeEquivalentTo(LatitudeRangeError);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Longitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Longitude)].ShouldBeEquivalentTo(LongitudeRangeError);
    }

    [Fact]
    public async Task WhenLatitudeMissing_ThenReturnsBadRequestWithValidationErrors()
    {
        HttpClient client = Fixture.CreateClient();
        Uri newUri = new("/validation", UriKind.Relative);
        var jsonBody = """
                          {
                              "longitude": 45
                          }
                          """;
        using var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

        // Act
        using HttpResponseMessage response = await client.PostAsync(newUri, content, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        ValidationProblemDetails? nullableValidationErrors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        ValidationProblemDetails validationErrors = nullableValidationErrors.ShouldNotBeNull();
        validationErrors.Errors.Count.ShouldBe(1);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Latitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Latitude)].ShouldBeEquivalentTo(LatitudeRequiredError);
    }

    [Fact]
    public async Task WhenLongitudeMissing_ThenReturnsBadRequestWithValidationErrors()
    {
        HttpClient client = Fixture.CreateClient();
        Uri newUri = new("/validation", UriKind.Relative);
        var jsonBody = """
                       {
                           "latitude": 42
                       }
                       """;
        using var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

        // Act
        using HttpResponseMessage response = await client.PostAsync(newUri, content, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        ValidationProblemDetails? nullableValidationErrors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        ValidationProblemDetails validationErrors = nullableValidationErrors.ShouldNotBeNull();
        validationErrors.Errors.Count.ShouldBe(1);
        validationErrors.Errors.ShouldContainKey(nameof(GeodeticEarthSurfaceDto.Longitude));
        validationErrors.Errors[nameof(GeodeticEarthSurfaceDto.Longitude)].ShouldBeEquivalentTo(LongitudeRequiredError);
    }
}
