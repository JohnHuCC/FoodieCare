namespace FoodieCare.ModernApi.Domain;

public enum PriceBand
{
    Unknown = 0,
    LessThan100,
    Between100And199,
    Between200And299,
    Between300And399,
    Between400And499,
    Above500
}

public static class PriceBandParser
{
    public static PriceBand Parse(string? value)
    {
        return value?.Trim() switch
        {
            "100_less" => PriceBand.LessThan100,
            "100_199" => PriceBand.Between100And199,
            "200_299" => PriceBand.Between200And299,
            "300_399" => PriceBand.Between300And399,
            "400_499" => PriceBand.Between400And499,
            "500_more" => PriceBand.Above500,
            _ => PriceBand.Unknown
        };
    }
}
