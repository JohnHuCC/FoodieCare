using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FoodieCare.ModernApi.Services;

public sealed class HybridPlacesService
{
    private readonly IEnumerable<IPlacesProvider> _providers;
    private readonly IMemoryCache _cache;
    private readonly FoodieCareOptions _options;

    public HybridPlacesService(
        IEnumerable<IPlacesProvider> providers,
        IMemoryCache cache,
        IOptions<FoodieCareOptions> options)
    {
        _providers = providers;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<HybridPlaceDto>> SearchAsync(
        HybridPlacesSearchRequest request,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeRequest(request);
        var cacheKey = BuildCacheKey(normalized);
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<HybridPlaceDto>? cached) && cached is not null)
        {
            return cached;
        }

        var merged = new List<HybridPlaceDto>();
        foreach (var provider in _providers)
        {
            if (provider is OsmPlacesProvider && !_options.HybridPlaces.UseOsm)
            {
                continue;
            }

            if (provider is GooglePlacesProvider && !_options.HybridPlaces.UseGoogle)
            {
                continue;
            }

            var limit = provider is GooglePlacesProvider
                ? Math.Min(normalized.Limit, _options.HybridPlaces.GoogleFetchLimit)
                : Math.Min(normalized.Limit, _options.HybridPlaces.OsmFetchLimit);
            var subReq = new HybridPlacesSearchRequest
            {
                Query = normalized.Query,
                Latitude = normalized.Latitude,
                Longitude = normalized.Longitude,
                RadiusKm = normalized.RadiusKm,
                Limit = limit
            };

            try
            {
                var result = await provider.SearchNearbyAsync(subReq, cancellationToken);
                merged.AddRange(result);
            }
            catch
            {
            }
        }

        var deduped = merged
            .GroupBy(x => BuildDedupKey(x), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderBy(x => x.DistanceKm).First())
            .OrderBy(x => x.DistanceKm)
            .Take(Math.Min(normalized.Limit, _options.HybridPlaces.MaxMergedResults))
            .ToList();

        _cache.Set(cacheKey, deduped, TimeSpan.FromMinutes(Math.Max(1, _options.HybridPlaces.CacheMinutes)));
        return deduped;
    }

    private static HybridPlacesSearchRequest NormalizeRequest(HybridPlacesSearchRequest request)
    {
        return new HybridPlacesSearchRequest
        {
            Query = request.Query?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusKm = Math.Clamp(request.RadiusKm, 1, 30),
            Limit = Math.Clamp(request.Limit, 1, 100)
        };
    }

    private static string BuildCacheKey(HybridPlacesSearchRequest request)
    {
        return $"hybrid:{request.Query?.ToLowerInvariant()}:{Math.Round(request.Latitude, 3)}:{Math.Round(request.Longitude, 3)}:{request.RadiusKm}:{request.Limit}";
    }

    private static string BuildDedupKey(HybridPlaceDto item)
    {
        var name = (item.Name ?? string.Empty).Trim().ToLowerInvariant();
        var lat = Math.Round(item.Latitude, 3);
        var lng = Math.Round(item.Longitude, 3);
        return $"{name}:{lat}:{lng}";
    }
}
