namespace FoodieCare.ModernApi.Models;

public sealed class StoreSearchRequest
{
    public string? Type { get; set; }
    public string? Price { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? DistanceKm { get; set; }
    public int? Limit { get; set; }
}
