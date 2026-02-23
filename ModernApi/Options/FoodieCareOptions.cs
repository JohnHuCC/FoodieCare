namespace FoodieCare.ModernApi.Options;

public sealed class FoodieCareOptions
{
    public const string SectionName = "FoodieCare";

    public string ConnectionString { get; set; } = string.Empty;
    public int DefaultDistanceKm { get; set; } = 5;
    public int DefaultCandidateCount { get; set; } = 5;
    public string RuleFile { get; set; } = "Rules/food_tree_rule.txt";
    public string AuthSecret { get; set; } = "change_me_to_a_long_random_secret";
    public int AuthTokenHours { get; set; } = 72;
}
