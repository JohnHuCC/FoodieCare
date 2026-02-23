namespace FoodieCare.ModernApi.Models;

public sealed class RecordClickRequest
{
    public required string StoreName { get; set; }
}

public sealed class RecommendationFeedbackRequest
{
    public int TasteId { get; set; }
    public int HungerId { get; set; }
    public int HotColdId { get; set; }
    public int EatModeId { get; set; }
    public int DistanceId { get; set; }
    public int PriceId { get; set; }
    public required string Type { get; set; }
    public bool Agree { get; set; } = true;
    public string? StoreName { get; set; }
}
