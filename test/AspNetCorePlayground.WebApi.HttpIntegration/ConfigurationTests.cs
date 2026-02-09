namespace AspNetCorePlayground.WebApi.HttpIntegration;

[Collection(ResumeAppCollectionFixture.Name)]
public sealed class ConfigurationTests
{

    private HttpTestFixture Fixture { get; }
    private ITestOutputHelper TestOutputHelper { get;  }

    public ConfigurationTests(HttpTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task WhenCallingConfigDictionary_ThenGetsTheTwoElements()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        IWebHostEnvironment webHostEnvironment = Fixture.Services.GetRequiredService<IWebHostEnvironment>();
        TestOutputHelper.WriteLine("Test code: Environment={0}", webHostEnvironment.EnvironmentName);

        // Act
        Dictionary<string, ConfigValueType>? dictionaryConfigurationResponse = await client.GetFromJsonAsync<Dictionary<string, ConfigValueType>>("/config/dictionary", TestContext.Current.CancellationToken);

        // Assert
        var dictionaryConfiguration = dictionaryConfigurationResponse.ShouldNotBeNull();
        dictionaryConfiguration.Count.ShouldBe(2);
    }

    // Duplicating instead of referencing the one in the ResumeService in order to test the "contract" not the existing implementation
#pragma warning disable CA1812
    internal sealed record ConfigValueType(int AnInteger, string Astring);
#pragma warning restore CA1812

    [Fact]
    public async Task WhenCallingConfigList_ThenGetsTheThreeElements()
    {
        // Arrange
        using HttpClient client = Fixture.CreateClient();
        IWebHostEnvironment webHostEnvironment = Fixture.Services.GetRequiredService<IWebHostEnvironment>();
        TestOutputHelper.WriteLine("Test code: Environment={0}", webHostEnvironment.EnvironmentName);

        // Act
        IList<string>? listConfigurationResponse = await client.GetFromJsonAsync<IList<string>>("/config/list", TestContext.Current.CancellationToken);

        // Assert
        var listConfiguration = listConfigurationResponse.ShouldNotBeNull();
        // This shows the danger with using lists.
        // Environment specific appsettings files do not REPLACE the entire list, only the
        listConfiguration.Count.ShouldBe(3);
        // You would think that these two would be the only elements, but you'll be wrong
        listConfiguration[0].ShouldBe("Fourth");
        listConfiguration[1].ShouldBe("Fifth");
        // The Third element from appsettings.json "survives"
        listConfiguration[2].ShouldBe("Third");
    }
}
