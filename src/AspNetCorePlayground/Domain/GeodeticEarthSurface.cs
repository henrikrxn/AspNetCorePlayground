using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Domain;

public class GeodeticEarthSurface
{
    [Required]
    [Range(-90.0,90.0)]
    public decimal Latitude { get; init; }

    [Required]
    [Range(-180.0,180.0)]
    public decimal Longitude { get; init; }
}
