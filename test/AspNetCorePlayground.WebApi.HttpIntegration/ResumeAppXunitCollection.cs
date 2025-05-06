namespace AspNetCorePlayground.WebApi.HttpIntegration;

// TODO THis is not necessary in xunit.v3 can use [assembly]
[CollectionDefinition(Name)]
public sealed class ResumeAppCollectionFixture : ICollectionFixture<HttpTestFixture>
{
    public const string Name = "AspNetCorePlayground HTTP tests";
}
