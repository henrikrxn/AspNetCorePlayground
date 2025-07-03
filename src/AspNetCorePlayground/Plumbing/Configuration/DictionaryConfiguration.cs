using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Plumbing.Configuration;

public class DictionaryConfiguration
{
    public const string  SectionName = "Dictionary";

    [MinLength(1)]
    public Dictionary<string, ConfigValueType> Items { get; } = [];
}

public class ConfigValueType
{
    [Required]
    [Range(1, 65536)]
    public int AnInteger { get; set; } = 0;

    [Required]
    public string Astring { get; set; } = string.Empty;
}
