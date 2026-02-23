using System.Text;
using System.Text.Json;
using FoodieCare.ModernApi.Models;

namespace FoodieCare.ModernApi.Services;

public sealed class OsmPlacesProvider : IPlacesProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OsmPlacesProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string Name => "osm";

    public async Task<IReadOnlyList<HybridPlaceDto>> SearchNearbyAsync(
        HybridPlacesSearchRequest request,
        CancellationToken cancellationToken)
    {
        var radiusMeters = Math.Max(200, request.RadiusKm * 1000);
        var q = request.Query?.Trim() ?? string.Empty;
        var filter = string.IsNullOrWhiteSpace(q)
            ? string.Empty
            : $"[~\"name\"~\"{EscapeRegex(q)}\",i]";

        var overpassQuery = $@"
[out:json][timeout:25];
(
  node[""amenity""=""restaurant""]{filter}(around:{radiusMeters},{request.Latitude},{request.Longitude});
  node[""amenity""=""cafe""]{filter}(around:{radiusMeters},{request.Latitude},{request.Longitude});
  way[""amenity""=""restaurant""]{filter}(around:{radiusMeters},{request.Latitude},{request.Longitude});
  way[""amenity""=""cafe""]{filter}(around:{radiusMeters},{request.Latitude},{request.Longitude});
);
out center {Math.Max(20, request.Limit)};
";

        var client = _httpClientFactory.CreateClient("overpass");
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["data"] = overpassQuery
        });
        using var response = await client.PostAsync("interpreter", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Array.Empty<HybridPlaceDto>();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("elements", out var elements) ||
            elements.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<HybridPlaceDto>();
        }

        var list = new List<HybridPlaceDto>();
        foreach (var el in elements.EnumerateArray())
        {
            var (lat, lng) = ReadCoordinate(el);
            if (!lat.HasValue || !lng.HasValue)
            {
                continue;
            }

            var tags = el.TryGetProperty("tags", out var t) ? t : default;
            var name = tags.ValueKind == JsonValueKind.Object && tags.TryGetProperty("name", out var n)
                ? n.GetString()
                : null;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var place = new HybridPlaceDto
            {
                Name = name!,
                Address = tags.ValueKind == JsonValueKind.Object && tags.TryGetProperty("addr:full", out var addr)
                    ? addr.GetString()
                    : null,
                Phone = tags.ValueKind == JsonValueKind.Object && tags.TryGetProperty("phone", out var phone)
                    ? phone.GetString()
                    : null,
                Latitude = lat.Value,
                Longitude = lng.Value,
                Rating = null,
                AveragePrice = null,
                DistanceKm = HaversineKm(request.Latitude, request.Longitude, lat.Value, lng.Value),
                Source = Name
            };
            list.Add(place);
        }

        return list
            .OrderBy(x => x.DistanceKm)
            .Take(request.Limit)
            .ToList();
    }

    private static (double? Lat, double? Lng) ReadCoordinate(JsonElement element)
    {
        if (element.TryGetProperty("lat", out var latEl) &&
            element.TryGetProperty("lon", out var lonEl) &&
            latEl.TryGetDouble(out var lat) &&
            lonEl.TryGetDouble(out var lon))
        {
            return (lat, lon);
        }

        if (element.TryGetProperty("center", out var center) &&
            center.TryGetProperty("lat", out var clat) &&
            center.TryGetProperty("lon", out var clon) &&
            clat.TryGetDouble(out var clatV) &&
            clon.TryGetDouble(out var clonV))
        {
            return (clatV, clonV);
        }

        return (null, null);
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

    private static string EscapeRegex(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if ("\\.[]{}()*+-?^$|".Contains(c))
            {
                sb.Append('\\');
            }
            sb.Append(c);
        }
        return sb.ToString();
    }
}
