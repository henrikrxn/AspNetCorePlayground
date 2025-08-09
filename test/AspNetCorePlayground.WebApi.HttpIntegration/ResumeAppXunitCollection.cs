namespace AspNetCorePlayground.WebApi.HttpIntegration;

[CollectionDefinition(Name)]
public sealed class ResumeAppCollectionFixture : ICollectionFixture<HttpTestFixture>
{
    public const string Name = "AspNetCorePlayground HTTP tests";
}
