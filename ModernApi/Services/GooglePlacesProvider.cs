using System.Text.Json;
using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Options;

namespace FoodieCare.ModernApi.Services;

public sealed class GooglePlacesProvider : IPlacesProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FoodieCareOptions _options;

    public GooglePlacesProvider(IHttpClientFactory httpClientFactory, IOptions<FoodieCareOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public string Name => "google";

    public async Task<IReadOnlyList<HybridPlaceDto>> SearchNearbyAsync(
        HybridPlacesSearchRequest request,
        CancellationToken cancellationToken)
    {
        var apiKey = _options.HybridPlaces.GoogleApiKey;
        if (!_options.HybridPlaces.UseGoogle || string.IsNullOrWhiteSpace(apiKey))
        {
            return Array.Empty<HybridPlaceDto>();
        }

        var client = _httpClientFactory.CreateClient("google-places");
        var radiusMeters = Math.Max(200, request.RadiusKm * 1000);
        var query = string.IsNullOrWhiteSpace(request.Query) ? "restaurant" : request.Query!.Trim();
        var url =
            $"nearbysearch/json?location={request.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{request.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&radius={radiusMeters}&keyword={Uri.EscapeDataString(query)}&key={Uri.EscapeDataString(apiKey)}";

        using var response = await client.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Array.Empty<HybridPlaceDto>();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<HybridPlaceDto>();
        }

        var list = new List<HybridPlaceDto>();
        foreach (var item in results.EnumerateArray())
        {
            var name = item.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (!item.TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("location", out var loc) ||
                !loc.TryGetProperty("lat", out var latEl) ||
                !loc.TryGetProperty("lng", out var lngEl) ||
                !latEl.TryGetDouble(out var lat) ||
                !lngEl.TryGetDouble(out var lng))
            {
                continue;
            }

            decimal? avgPrice = null;
            if (item.TryGetProperty("price_level", out var priceEl) && priceEl.TryGetInt32(out var level))
            {
                avgPrice = level switch
                {
                    0 => 80m,
                    1 => 150m,
                    2 => 300m,
                    3 => 500m,
                    4 => 700m,
                    _ => null
                };
            }

            double? rating = null;
            if (item.TryGetProperty("rating", out var ratingEl) && ratingEl.TryGetDouble(out var rv))
            {
                rating = rv;
            }

            var place = new HybridPlaceDto
            {
                Name = name!,
                Address = item.TryGetProperty("vicinity", out var vic) ? vic.GetString() : null,
                Phone = null,
                Latitude = lat,
                Longitude = lng,
                Rating = rating,
                AveragePrice = avgPrice,
                DistanceKm = HaversineKm(request.Latitude, request.Longitude, lat, lng),
                Source = Name
            };
            list.Add(place);
        }

        return list
            .OrderBy(x => x.DistanceKm)
            .Take(request.Limit)
            .ToList();
    }

    private static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
    {
        const double r = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLng = (lng2 - lng1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return r * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
