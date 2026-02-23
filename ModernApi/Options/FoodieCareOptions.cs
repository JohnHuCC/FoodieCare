namespace FoodieCare.ModernApi.Options;

public sealed class FoodieCareOptions
{
    public sealed class RankingOptions
    {
        public bool Enabled { get; set; } = true;
        public double DistanceWeight { get; set; } = 0.5;
        public double PriceWeight { get; set; } = 0.2;
        public double RatingWeight { get; set; } = 0.2;
        public double TypeWeight { get; set; } = 0.1;
        public double ExplorationWeight { get; set; } = 0.05;
        public int ExplorationBucketCount { get; set; } = 9;
        public int DbFetchMultiplier { get; set; } = 4;
    }

    public sealed class HybridPlacesOptions
    {
        public bool Enabled { get; set; } = true;
        public bool UseOsm { get; set; } = true;
        public bool UseGoogle { get; set; } = false;
        public string GoogleApiKey { get; set; } = string.Empty;
        public int CacheMinutes { get; set; } = 15;
        public int MaxMergedResults { get; set; } = 60;
        public int OsmFetchLimit { get; set; } = 60;
        public int GoogleFetchLimit { get; set; } = 40;
    }

    public const string SectionName = "FoodieCare";

    public string ConnectionString { get; set; } = string.Empty;
    public int DefaultDistanceKm { get; set; } = 5;
    public int DefaultCandidateCount { get; set; } = 5;
    public string RuleFile { get; set; } = "Rules/food_tree_rule.txt";
    public string AuthSecret { get; set; } = "change_me_to_a_long_random_secret";
    public int AuthTokenHours { get; set; } = 72;
    public int SeedScaleFactor { get; set; } = 1;
    public bool EnableSyntheticNearbyFallback { get; set; } = false;
    public int SyntheticFallbackCount { get; set; } = 20;
    public RankingOptions Ranking { get; set; } = new();
    public HybridPlacesOptions HybridPlaces { get; set; } = new();
    public string ActiveRankingProfile { get; set; } = "balanced";
    public Dictionary<string, RankingOptions> RankingProfiles { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["balanced"] = new RankingOptions(),
        ["conservative"] = new RankingOptions
        {
            Enabled = true,
            DistanceWeight = 0.6,
            PriceWeight = 0.22,
            RatingWeight = 0.12,
            TypeWeight = 0.06,
            ExplorationWeight = 0.02,
            ExplorationBucketCount = 9,
            DbFetchMultiplier = 3
        },
        ["explore"] = new RankingOptions
        {
            Enabled = true,
            DistanceWeight = 0.38,
            PriceWeight = 0.15,
            RatingWeight = 0.17,
            TypeWeight = 0.08,
            ExplorationWeight = 0.22,
            ExplorationBucketCount = 13,
            DbFetchMultiplier = 6
        }
    };
}
