namespace ResumeService.Test.WebApi.HttpIntegration;

[CollectionDefinition(Name)]
public sealed class ResumeAppCollection : ICollectionFixture<HttpTestFixture>
{
    public const string Name = "ResumeApp HTTP tests";
}