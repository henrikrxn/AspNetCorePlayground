using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Plumbing.Configuration;

public class ListConfiguration
{
    public const string  SectionName = "List";

    [MinLength(1)]
    public IList<string> Items { get; } = [];
}
