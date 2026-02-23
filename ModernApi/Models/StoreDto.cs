namespace FoodieCare.ModernApi.Models;

public sealed class StoreDto
{
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Rating { get; set; }
    public decimal? AveragePrice { get; set; }
    public double DistanceKm { get; set; }
}
