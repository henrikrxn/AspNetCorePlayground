using Microsoft.Net.Http.Headers;

namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class CorsTests
{
    public CorsTests(HttpTestFixture fixture)
    {
        Fixture = fixture;
    }

    private HttpTestFixture Fixture { get; }

    [Fact]
    public async Task WhenOptionsRequestOriginIsAllowedOrigin_TheOptionsReplyContainsSentOriginInAccessControlAllowOrigin()
    {
        HttpClient client = Fixture.CreateClient();
        using HttpRequestMessage request = CreatePreflightRequestWithRequiredCorsHeaders();

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _ = response.EnsureSuccessStatusCode();
        IEnumerable<string> allowedOriginsHeaders = response.Headers.GetValues(HeaderNames.AccessControlAllowOrigin);
        allowedOriginsHeaders.ShouldHaveSingleItem().ShouldBe(DefaultOrigin);
    }

    // TODO Add happy and rainy paths with allowed request headers

    [Fact]
    public async Task WhenOriginNotAllowed_ThenOptionsRequestFailsBecauseAccessControlAllowOriginNotPresent()
    {
        HttpClient client = Fixture.CreateClient();
        const string originNotInAllowedOrigins = "https://g.h.i";
        using HttpRequestMessage request = CreatePreflightRequestWithRequiredCorsHeaders(origin: originNotInAllowedOrigins);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        // No AccessControlAllowOrigin => Failure
        response.Headers.Contains(HeaderNames.AccessControlAllowOrigin).ShouldBeFalse();
    }

    // This is an allowed origin
    private const string DefaultOrigin = "https://a.b.c";

    private static HttpRequestMessage CreatePreflightRequestWithRequiredCorsHeaders(string partialRequestUrl = "/weatherforecast", string requestMethod = "GET", string origin = DefaultOrigin)
    {
        var optionsRequest = new HttpRequestMessage(HttpMethod.Options, partialRequestUrl);
        // CORS requests must include the Origin header, see https://fetch.spec.whatwg.org/#http-requests
        optionsRequest.Headers.Add(HeaderNames.Origin, origin);
        // Pre-flight requests must include Access-Control-Request-Method, see https://fetch.spec.whatwg.org/#http-requests
        optionsRequest.Headers.Add(HeaderNames.AccessControlRequestMethod, requestMethod);
        var originUri = new Uri(origin);

        return optionsRequest;
    }
}
