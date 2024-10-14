namespace ResumeService.Test.WebApi.HttpIntegration;

[CollectionDefinition(Name)]
public sealed class ResumeAppCollectionFixture : ICollectionFixture<HttpTestFixture>
{
    public const string Name = "ResumeApp HTTP tests";
}
