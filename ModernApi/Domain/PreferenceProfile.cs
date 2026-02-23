namespace FoodieCare.ModernApi.Domain;

public sealed record PreferenceProfile(
    string Price,
    string EatMode,
    string Hunger,
    string Distance,
    string HotCold,
    string? Taste = null);
