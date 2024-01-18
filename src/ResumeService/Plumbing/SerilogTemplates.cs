namespace ResumeService.Plumbing;

public static class SerilogTemplates
{
    public static readonly string IncludesProperties = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}";
}
