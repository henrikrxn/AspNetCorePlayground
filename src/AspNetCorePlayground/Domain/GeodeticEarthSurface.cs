using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Domain;

public class GeodeticEarthSurface
{
    public decimal Latitude { get; init; }

    public decimal Longitude { get; init; }
}
