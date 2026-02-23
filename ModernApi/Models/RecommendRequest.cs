namespace FoodieCare.ModernApi.Models;

public sealed class RecommendRequest
{
    public int? UserId { get; set; }
    public required string Price { get; set; }
    public required string EatMode { get; set; }
    public required string Hunger { get; set; }
    public required string Distance { get; set; }
    public required string HotCold { get; set; }
    public string? Taste { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
