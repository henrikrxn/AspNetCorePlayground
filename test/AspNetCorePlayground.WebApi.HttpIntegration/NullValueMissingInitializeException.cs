namespace AspNetCorePlayground.WebApi.HttpIntegration;

internal sealed class NullValueMissingInitializeException : Exception
{
    public NullValueMissingInitializeException() { }
    public NullValueMissingInitializeException(string message) : base(message) { }
    public NullValueMissingInitializeException(string message, Exception inner) : base(message, inner) { }
}
