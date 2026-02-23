namespace FoodieCare.ModernApi.Domain;

public sealed class RuleNode
{
    public string Feature { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? Prediction { get; init; }
    public List<RuleNode> Children { get; } = new();
}
