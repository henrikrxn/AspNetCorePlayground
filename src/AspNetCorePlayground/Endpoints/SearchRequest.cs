using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Endpoints;

public sealed record SearchRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductNumber { get; init; }
}
