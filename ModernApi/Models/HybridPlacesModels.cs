namespace FoodieCare.ModernApi.Models;

public sealed class HybridPlacesSearchRequest
{
    public string? Query { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RadiusKm { get; set; } = 5;
    public int Limit { get; set; } = 40;
}

public sealed class HybridPlaceDto
{
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Rating { get; set; }
    public decimal? AveragePrice { get; set; }
    public double DistanceKm { get; set; }
    public string Source { get; set; } = string.Empty;
}
