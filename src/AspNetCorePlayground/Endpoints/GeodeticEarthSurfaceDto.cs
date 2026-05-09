using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Endpoints;

public class GeodeticEarthSurfaceDto
{
    [Required]
    [Range(-90.0,90.0)]
    public decimal? Latitude { get; init; }

    [Required]
    [Range(-180.0,180.0)]
    public decimal? Longitude { get; init; }
}
