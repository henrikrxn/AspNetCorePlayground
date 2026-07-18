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

    // Duplicating instead of referencing the ones in the Project in order to test the "contract" not the existing implementation
#pragma warning disable CA1812
    internal sealed record SearchRequest(int ProductNumber);
    internal sealed record SearchResponse(string Sku);

#pragma warning restore CA1812

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
}
