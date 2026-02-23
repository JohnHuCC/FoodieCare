namespace FoodieCare.ModernApi.Models;

public sealed class RecommendOptionsResponse
{
    public required string PrimaryType { get; set; }
    public required IReadOnlyList<string> CandidateTypes { get; set; }
}
