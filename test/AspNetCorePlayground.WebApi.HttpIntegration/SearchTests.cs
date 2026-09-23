using System.Net;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;

namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class SearchTests
{

    private HttpTestFixture Fixture { get; }
    private ITestOutputHelper TestOutputHelper { get;  }

    public SearchTests(HttpTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task WhenCallingSearch_ThenGetProductNumber()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        using var queryRequest = new HttpRequestMessage(new HttpMethod("QUERY"),"/search");
        queryRequest.Content = JsonContent.Create(new SearchRequest(42));

        // Act
        HttpResponseMessage response = await client.SendAsync(queryRequest, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        SearchResponse? readFromJson = await response.Content.ReadFromJsonAsync<SearchResponse>(TestContext.Current.CancellationToken);
        SearchResponse searchResponse = readFromJson.ShouldNotBeNull();
        searchResponse.Sku.ShouldBe("Product42");
    }

    [Fact]
    public async Task WhenCallingSearchWithContentTypeXml_ThenFailsWithUnsupportedMediaType()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        var requestData = new SearchRequest(42);
        var serializer = new XmlSerializer(typeof(SearchRequest));

        await using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, requestData);
        using var queryRequest = new HttpRequestMessage(new HttpMethod("QUERY"),"/search");
        queryRequest.Content = new StringContent(stringWriter.ToString(), Encoding.UTF8, MediaTypeNames.Application.Xml);

        // Act
        HttpResponseMessage response = await client.SendAsync(queryRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnsupportedMediaType);
    }
}

// Duplicating instead of referencing the ones in the Project in order to test the "contract" not the existing implementation
#pragma warning disable CA1812
public sealed record SearchRequest(int ProductNumber)
{
    public SearchRequest() : this(-1)
    {
    }
}
internal sealed record SearchResponse(string Sku);

#pragma warning restore CA1812
