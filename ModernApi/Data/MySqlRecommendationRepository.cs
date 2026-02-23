using System.Text;
using System.Text.Json;
using FoodieCare.ModernApi.Domain;
using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace FoodieCare.ModernApi.Data;

public sealed class MySqlRecommendationRepository : IRecommendationRepository
{
    private sealed class SeedStore
    {
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double? Rating { get; set; }
        public decimal? AvgPrice { get; set; }
        public string C3 { get; set; } = string.Empty;
        public string? C4 { get; set; }
    }

    private static readonly HashSet<string> UserDataColumns = new(StringComparer.Ordinal)
    {
        "鍋類",
        "日式料理",
        "咖啡、簡餐、下午茶",
        "buffet自助餐",
        "烘焙、甜點、零食",
        "小吃",
        "燒烤類",
        "早餐/早午餐",
        "韓式料理",
        "亞洲料理(港、泰、印、星、馬)",
        "異國料理(歐洲、美洲)",
        "中式料理",
        "主題特色餐廳",
        "其他美食",
        "冰品、飲料、甜湯",
        "素食",
        "速食料理"
    };

    private static readonly IReadOnlyDictionary<string, string> UserDataColumnAlias = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["早餐"] = "早餐/早午餐",
        ["亞洲料理"] = "亞洲料理(港、泰、印、星、馬)",
        ["咖啡、簡餐、茶"] = "咖啡、簡餐、下午茶",
        ["下午茶"] = "咖啡、簡餐、下午茶",
        ["異國料理"] = "異國料理(歐洲、美洲)"
    };

    private readonly string _connectionString;
    private readonly IReadOnlyList<SeedStore> _seedStores;
    private readonly FoodieCareOptions _options;
    private readonly FoodieCareOptions.RankingOptions _ranking;
    private readonly string _activeRankingProfile;

    public MySqlRecommendationRepository(IOptions<FoodieCareOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value;
        _connectionString = _options.ConnectionString;
        _seedStores = ExpandSeedStores(LoadSeedStores(env.ContentRootPath), Math.Max(1, _options.SeedScaleFactor));
        _ranking = ResolveRanking(_options, out _activeRankingProfile);
    }

    public Task<IReadOnlyList<StoreDto>> BrowseStoresAsync(
        string? type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken)
    {
        return QueryStoresAsync(type, priceBand, latitude, longitude, maxDistanceKm, limit, cancellationToken);
    }

    public async Task<IReadOnlyList<StoreDto>> SearchStoresAsync(
        string type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken)
    {
        return await QueryStoresAsync(type, priceBand, latitude, longitude, maxDistanceKm, limit, cancellationToken);
    }

    public int DebugGetSeedCount() => _seedStores.Count;
    public string DebugGetActiveRankingProfile() => _activeRankingProfile;

    public async Task<IReadOnlyList<string>> GetNearbyTypesAsync(
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken)
    {
        const string distanceSql = "(6371 * ACOS(COS(RADIANS(@lat)) * COS(RADIANS(lat)) * COS(RADIANS(lng) - RADIANS(@lng)) + SIN(RADIANS(@lat)) * SIN(RADIANS(lat))))";
        var sql = $@"
SELECT `c3`, MIN({distanceSql}) AS min_distance
FROM `food_back`
WHERE `c3` IS NOT NULL AND `c3` <> ''
GROUP BY `c3`
HAVING min_distance < @maxDistance
ORDER BY min_distance ASC
LIMIT @limit;";

        try
        {
            var result = new List<string>();
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@lat", latitude);
            command.Parameters.AddWithValue("@lng", longitude);
            command.Parameters.AddWithValue("@maxDistance", maxDistanceKm);
            command.Parameters.AddWithValue("@limit", limit);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (reader.IsDBNull(0))
                {
                    continue;
                }

                var type = reader.GetString(0);
                if (!string.IsNullOrWhiteSpace(type))
                {
                    result.Add(type);
                }
            }

            if (result.Count > 0)
            {
                return result;
            }
        }
        catch (MySqlException)
        {
        }

        var nearbyFromSeed = _seedStores
            .Select(x => new
            {
                x.C3,
                Distance = HaversineKm(latitude, longitude, x.Lat, x.Lng)
            })
            .Where(x => x.Distance <= maxDistanceKm && !string.IsNullOrWhiteSpace(x.C3))
            .OrderBy(x => x.Distance)
            .Select(x => x.C3)
            .Distinct(StringComparer.Ordinal)
            .Take(limit)
            .ToList();

        if (nearbyFromSeed.Count > 0)
        {
            return nearbyFromSeed;
        }

        if (_options.EnableSyntheticNearbyFallback)
        {
            return _seedStores
                .Where(x => !string.IsNullOrWhiteSpace(x.C3))
                .Select(x => x.C3)
                .Distinct(StringComparer.Ordinal)
                .Take(limit)
                .ToList();
        }

        return Array.Empty<string>();
    }

    private async Task<IReadOnlyList<StoreDto>> QueryStoresAsync(
        string? type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken)
    {
        const string distanceSql = "(6371 * ACOS(COS(RADIANS(@lat)) * COS(RADIANS(lat)) * COS(RADIANS(lng) - RADIANS(@lng)) + SIN(RADIANS(@lat)) * SIN(RADIANS(lat))))";

        var whereBuilder = new StringBuilder("1=1");
        if (!string.IsNullOrWhiteSpace(type))
        {
            whereBuilder.Append(" AND (`c3` = @type OR `c4` = @type)");
        }
        switch (priceBand)
        {
            case PriceBand.LessThan100:
                whereBuilder.Append(" AND `平均價格` < 100");
                break;
            case PriceBand.Between100And199:
                whereBuilder.Append(" AND `平均價格` >= 100 AND `平均價格` < 200");
                break;
            case PriceBand.Between200And299:
                whereBuilder.Append(" AND `平均價格` >= 200 AND `平均價格` < 300");
                break;
            case PriceBand.Between300And399:
                whereBuilder.Append(" AND `平均價格` >= 300 AND `平均價格` < 400");
                break;
            case PriceBand.Between400And499:
                whereBuilder.Append(" AND `平均價格` >= 400 AND `平均價格` < 500");
                break;
            case PriceBand.Above500:
                whereBuilder.Append(" AND `平均價格` >= 500");
                break;
        }

        var fetchLimit = Math.Max(limit, limit * Math.Max(1, _ranking.DbFetchMultiplier));
        var sql = $@"
SELECT
    `圖片`,
    `商家名稱`,
    `電話`,
    `lat`,
    `lng`,
    `point`,
    `平均價格`,
    {distanceSql} AS DISTANCE
FROM `food_back`
WHERE {whereBuilder}
HAVING DISTANCE < @maxDistance AND DISTANCE > 0.0001
ORDER BY DISTANCE ASC
LIMIT @limit;";

        var result = new List<StoreDto>();

        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new MySqlCommand(sql, connection);
            if (!string.IsNullOrWhiteSpace(type))
            {
                command.Parameters.AddWithValue("@type", type);
            }
            command.Parameters.AddWithValue("@lat", latitude);
            command.Parameters.AddWithValue("@lng", longitude);
            command.Parameters.AddWithValue("@maxDistance", maxDistanceKm);
            command.Parameters.AddWithValue("@limit", fetchLimit);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new StoreDto
                {
                    ImageUrl = reader.IsDBNull(0) ? null : reader.GetString(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Latitude = reader.IsDBNull(3) ? 0d : Convert.ToDouble(reader.GetValue(3)),
                    Longitude = reader.IsDBNull(4) ? 0d : Convert.ToDouble(reader.GetValue(4)),
                    Rating = reader.IsDBNull(5) ? null : Convert.ToDouble(reader.GetValue(5)),
                    AveragePrice = reader.IsDBNull(6) ? null : Convert.ToDecimal(reader.GetValue(6)),
                    DistanceKm = reader.IsDBNull(7) ? 0d : Convert.ToDouble(reader.GetValue(7))
                });
            }
        }
        catch (MySqlException)
        {
            if (_seedStores.Count > 0)
            {
                return QuerySeedStores(type, priceBand, latitude, longitude, maxDistanceKm, limit);
            }

            throw;
        }

        if (result.Count == 0 && _seedStores.Count > 0)
        {
            return QuerySeedStores(type, priceBand, latitude, longitude, maxDistanceKm, limit);
        }

        return RankStores(result, type, priceBand, maxDistanceKm, limit);
    }

    private IReadOnlyList<StoreDto> QuerySeedStores(
        string? type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit)
    {
        var projected = _seedStores
            .Select(x => new
            {
                Store = new StoreDto
                {
                    Name = x.Name,
                    Phone = x.Phone,
                    Latitude = x.Lat,
                    Longitude = x.Lng,
                    Rating = x.Rating,
                    AveragePrice = x.AvgPrice,
                    DistanceKm = HaversineKm(latitude, longitude, x.Lat, x.Lng)
                },
                TypeMatch = string.IsNullOrWhiteSpace(type) ||
                            string.Equals(x.C3, type, StringComparison.Ordinal) ||
                            string.Equals(x.C4, type, StringComparison.Ordinal),
                PriceMatch = MatchesPriceBand(x.AvgPrice, priceBand)
            })
            .Where(x => x.Store.DistanceKm > 0.0001)
            .OrderBy(x => x.Store.DistanceKm)
            .ToList();

        var strict = projected
            .Where(x => x.Store.DistanceKm < maxDistanceKm && x.TypeMatch && x.PriceMatch)
            .Select(x => x.Store)
            .ToList();

        strict = RankStores(strict, type, priceBand, maxDistanceKm, limit);

        if (strict.Count > 0)
        {
            return strict;
        }

        // Keep distance strict: never return stores outside requested radius.
        var relaxType = projected
            .Where(x => x.Store.DistanceKm < maxDistanceKm && x.TypeMatch)
            .Select(x => x.Store)
            .ToList();

        relaxType = RankStores(relaxType, type, priceBand, maxDistanceKm, limit);

        if (relaxType.Count > 0)
        {
            return relaxType;
        }

        var relaxDistance = projected
            .Where(x => x.Store.DistanceKm < maxDistanceKm)
            .Select(x => x.Store)
            .ToList();

        relaxDistance = RankStores(relaxDistance, type, priceBand, maxDistanceKm, limit);

        if (relaxDistance.Count > 0)
        {
            return relaxDistance;
        }

        if (_options.EnableSyntheticNearbyFallback)
        {
            return CreateSyntheticNearbyStores(type, priceBand, latitude, longitude, maxDistanceKm, limit);
        }

        return Array.Empty<StoreDto>();
    }

    private List<StoreDto> RankStores(
        List<StoreDto> stores,
        string? requestedType,
        PriceBand priceBand,
        int distanceWindowKm,
        int limit)
    {
        if (stores.Count == 0)
        {
            return stores;
        }

        if (!_ranking.Enabled)
        {
            return stores
                .OrderBy(x => x.DistanceKm)
                .Take(limit)
                .ToList();
        }

        var ranked = stores
            .Select(x =>
            {
                var score = ComputeScore(
                    x,
                    requestedType,
                    priceBand,
                    distanceWindowKm,
                    _ranking);
                x.Score = score;
                return x;
            })
            .OrderByDescending(x => x.Score ?? 0d)
            .ThenBy(x => x.DistanceKm)
            .Take(limit)
            .ToList();

        return ranked;
    }

    private static double ComputeScore(
        StoreDto store,
        string? requestedType,
        PriceBand priceBand,
        int distanceWindowKm,
        FoodieCareOptions.RankingOptions ranking)
    {
        var distanceBase = Math.Max(1, distanceWindowKm);
        var distanceScore = 1.0 - Math.Min(1.0, store.DistanceKm / distanceBase);

        var priceScore = MatchPriceScore(store.AveragePrice, priceBand);
        var ratingScore = store.Rating.HasValue ? Math.Clamp(store.Rating.Value / 5.0, 0.0, 1.0) : 0.6;
        var typeScore = string.IsNullOrWhiteSpace(requestedType) ? 0.7 : 1.0;

        var bucketCount = Math.Max(2, ranking.ExplorationBucketCount);
        var exploreSeed = Math.Abs((store.Name ?? string.Empty).GetHashCode(StringComparison.Ordinal));
        var exploreScore = (exploreSeed % bucketCount) / (double)(bucketCount - 1);

        return (ranking.DistanceWeight * distanceScore)
               + (ranking.PriceWeight * priceScore)
               + (ranking.RatingWeight * ratingScore)
               + (ranking.TypeWeight * typeScore)
               + (ranking.ExplorationWeight * exploreScore);
    }

    private static double MatchPriceScore(decimal? avgPrice, PriceBand band)
    {
        if (!avgPrice.HasValue || band == PriceBand.Unknown)
        {
            return 0.7;
        }

        return band switch
        {
            PriceBand.LessThan100 => avgPrice.Value < 100m ? 1.0 : 0.25,
            PriceBand.Between100And199 => avgPrice.Value >= 100m && avgPrice.Value < 200m ? 1.0 : 0.25,
            PriceBand.Between200And299 => avgPrice.Value >= 200m && avgPrice.Value < 300m ? 1.0 : 0.25,
            PriceBand.Between300And399 => avgPrice.Value >= 300m && avgPrice.Value < 400m ? 1.0 : 0.25,
            PriceBand.Between400And499 => avgPrice.Value >= 400m && avgPrice.Value < 500m ? 1.0 : 0.25,
            PriceBand.Above500 => avgPrice.Value >= 500m ? 1.0 : 0.25,
            _ => 0.7
        };
    }

    private IReadOnlyList<StoreDto> CreateSyntheticNearbyStores(
        string? type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit)
    {
        var targetCount = Math.Max(1, Math.Min(limit, _options.SyntheticFallbackCount));
        var radiusKm = Math.Max(0.6, maxDistanceKm * 0.9);

        var templates = _seedStores
            .Where(x =>
                (string.IsNullOrWhiteSpace(type) ||
                 string.Equals(x.C3, type, StringComparison.Ordinal) ||
                 string.Equals(x.C4, type, StringComparison.Ordinal)) &&
                MatchesPriceBand(x.AvgPrice, priceBand))
            .Take(targetCount)
            .ToList();

        if (templates.Count == 0)
        {
            templates = _seedStores.Take(targetCount).ToList();
        }

        var list = new List<StoreDto>(templates.Count);
        for (var i = 0; i < templates.Count; i++)
        {
            var t = templates[i];
            var seed = Math.Abs(HashCode.Combine(t.Name, latitude, longitude, i));
            var angle = (seed % 360) * Math.PI / 180.0;
            var distance = Math.Max(0.15, ((seed % 1000) / 1000.0) * radiusKm);

            var latDelta = (distance / 111.0) * Math.Cos(angle);
            var lngDenominator = Math.Max(0.2, Math.Cos(latitude * Math.PI / 180.0));
            var lngDelta = (distance / (111.0 * lngDenominator)) * Math.Sin(angle);

            var lat = Math.Clamp(latitude + latDelta, -89.9, 89.9);
            var lng = Math.Clamp(longitude + lngDelta, -179.9, 179.9);
            var actualDistance = HaversineKm(latitude, longitude, lat, lng);

            list.Add(new StoreDto
            {
                Name = $"{t.Name} (Nearby)",
                Phone = t.Phone,
                Latitude = lat,
                Longitude = lng,
                Rating = t.Rating,
                AveragePrice = t.AvgPrice,
                DistanceKm = actualDistance
            });
        }

        return RankStores(list, type, priceBand, maxDistanceKm, limit);
    }

    private static FoodieCareOptions.RankingOptions ResolveRanking(FoodieCareOptions options, out string activeProfile)
    {
        var profile = options.ActiveRankingProfile?.Trim();
        if (!string.IsNullOrWhiteSpace(profile) &&
            options.RankingProfiles is not null &&
            options.RankingProfiles.TryGetValue(profile, out var ranking) &&
            ranking is not null)
        {
            activeProfile = profile;
            return ranking;
        }

        activeProfile = "default";
        return options.Ranking;
    }

    private static bool MatchesPriceBand(decimal? avgPrice, PriceBand band)
    {
        if (!avgPrice.HasValue || band == PriceBand.Unknown)
        {
            return true;
        }

        return band switch
        {
            PriceBand.LessThan100 => avgPrice.Value < 100m,
            PriceBand.Between100And199 => avgPrice.Value >= 100m && avgPrice.Value < 200m,
            PriceBand.Between200And299 => avgPrice.Value >= 200m && avgPrice.Value < 300m,
            PriceBand.Between300And399 => avgPrice.Value >= 300m && avgPrice.Value < 400m,
            PriceBand.Between400And499 => avgPrice.Value >= 400m && avgPrice.Value < 500m,
            PriceBand.Above500 => avgPrice.Value >= 500m,
            _ => true
        };
    }

    private static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
    {
        const double r = 6371.0;
        var dLat = ToRad(lat2 - lat1);
        var dLng = ToRad(lng2 - lng1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    private static double ToRad(double value) => value * Math.PI / 180.0;

    private static IReadOnlyList<SeedStore> LoadSeedStores(string contentRootPath)
    {
        try
        {
            var path = Path.Combine(contentRootPath, "Data", "global_stores.json");
            if (!File.Exists(path))
            {
                path = Path.Combine(contentRootPath, "Data", "london_stores.json");
            }

            if (!File.Exists(path))
            {
                return Array.Empty<SeedStore>();
            }

            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<SeedStore>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new List<SeedStore>();
        }
        catch
        {
            return Array.Empty<SeedStore>();
        }
    }

    private static IReadOnlyList<SeedStore> ExpandSeedStores(IReadOnlyList<SeedStore> source, int scaleFactor)
    {
        if (source.Count == 0 || scaleFactor <= 1)
        {
            return source;
        }

        var expanded = new List<SeedStore>(source.Count * scaleFactor);
        expanded.AddRange(source);

        for (var round = 1; round < scaleFactor; round++)
        {
            foreach (var item in source)
            {
                var seed = Math.Abs(HashCode.Combine(item.Name, item.C3 ?? string.Empty, item.C4 ?? string.Empty, round));
                var latOffset = ((seed % 2001) - 1000) / 100000.0; // about +/- 0.01
                var lngOffset = (((seed / 2001) % 2001) - 1000) / 100000.0;

                var priceDelta = (seed % 9) - 4; // -4..+4
                decimal? adjustedPrice = item.AvgPrice.HasValue
                    ? Math.Max(1m, item.AvgPrice.Value + priceDelta)
                    : null;

                double? adjustedRating = item.Rating.HasValue
                    ? Math.Clamp(item.Rating.Value + (((seed / 17) % 5) - 2) * 0.05, 3.0, 5.0)
                    : null;

                expanded.Add(new SeedStore
                {
                    Name = $"{item.Name} #{round + 1}",
                    Phone = item.Phone,
                    Lat = Math.Clamp(item.Lat + latOffset, -89.9, 89.9),
                    Lng = Math.Clamp(item.Lng + lngOffset, -179.9, 179.9),
                    Rating = adjustedRating,
                    AvgPrice = adjustedPrice,
                    C3 = item.C3 ?? string.Empty,
                    C4 = item.C4
                });
            }
        }

        return expanded;
    }

    public async Task<IReadOnlyList<string>> GetAssociationCandidatesAsync(
        string type,
        int limit,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return Array.Empty<string>();
        }

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string existsSql = @"
SELECT COUNT(*)
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'food_association'
  AND column_name = @col;";

        await using (var existsCmd = new MySqlCommand(existsSql, connection))
        {
            existsCmd.Parameters.AddWithValue("@col", type);
            var count = Convert.ToInt32(await existsCmd.ExecuteScalarAsync(cancellationToken));
            if (count == 0)
            {
                return Array.Empty<string>();
            }
        }

        var escapedColumn = type.Replace("`", "``", StringComparison.Ordinal);
        var sql = $"SELECT * FROM `food_association` WHERE `{escapedColumn}` = 2 LIMIT 1;";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return Array.Empty<string>();
        }

        var scores = new List<(string Type, int Score)>();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            if (name == type || string.Equals(name, "id", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (reader.IsDBNull(i))
            {
                continue;
            }

            var score = Convert.ToInt32(reader.GetValue(i));
            if (score > 0)
            {
                scores.Add((name, score));
            }
        }

        return scores
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Type, StringComparer.Ordinal)
            .Take(limit)
            .Select(x => x.Type)
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetRecentUserTypesAsync(
        int userId,
        int limit,
        CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT fb.`c3`
FROM `food_back` fb
INNER JOIN (
    SELECT `商家名稱`
    FROM `user_click`
    WHERE `ID` = @userId
    ORDER BY `update_time` DESC
    LIMIT 100
) uc ON uc.`商家名稱` = fb.`商家名稱`;";

        var counts = new Dictionary<string, int>(StringComparer.Ordinal);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader.IsDBNull(0))
            {
                continue;
            }

            var type = reader.GetString(0);
            counts[type] = counts.TryGetValue(type, out var n) ? n + 1 : 1;
        }

        return counts
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key, StringComparer.Ordinal)
            .Take(limit)
            .Select(x => x.Key)
            .ToList();
    }

    public async Task<UserIdentity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        const string sql = "SELECT `ID`, `帳號` FROM `account` WHERE `帳號` = @username LIMIT 1;";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserIdentity
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1)
        };
    }

    public async Task<(int UserId, string Username)?> ValidateUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT `ID`, `帳號`
FROM `account`
WHERE `帳號` = @username AND `密碼` = @password
LIMIT 1;";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return (reader.GetInt32(0), reader.GetString(1));
    }

    public async Task<(int UserId, string Username)> CreateUserAsync(string username, string password, int gender, int age, CancellationToken cancellationToken)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            const string insertAccount = @"
INSERT INTO `account`
(`帳號`, `密碼`, `性別`, `年齡`, `通常吃飯價位位於`, `願意走多少距離`, `吃冷食還是熱食`, `通常與誰用餐`, `吃多少`)
VALUES
(@username, @password, @gender, @age, '', '', '', '', 0);";

            await using (var insertCmd = new MySqlCommand(insertAccount, connection, tx))
            {
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@password", password);
                insertCmd.Parameters.AddWithValue("@gender", gender);
                insertCmd.Parameters.AddWithValue("@age", age);
                await insertCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            int userId;
            const string lastIdSql = "SELECT LAST_INSERT_ID();";
            await using (var lastIdCmd = new MySqlCommand(lastIdSql, connection, tx))
            {
                userId = Convert.ToInt32(await lastIdCmd.ExecuteScalarAsync(cancellationToken) ?? 0);
            }
            if (userId <= 0)
            {
                throw new InvalidOperationException("Failed to create user.");
            }

            const string insertProfile = "INSERT INTO `user_data` (`ID`) VALUES (@userId);";
            await using (var profileCmd = new MySqlCommand(insertProfile, connection, tx))
            {
                profileCmd.Parameters.AddWithValue("@userId", userId);
                await profileCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);
            return (userId, username);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RecordUserClickAsync(int userId, string storeName, CancellationToken cancellationToken)
    {
        const string sql = @"
INSERT INTO `user_click` (`ID`, `商家名稱`, `count`)
VALUES (@userId, @storeName, 1)
ON DUPLICATE KEY UPDATE
`count` = `count` + 1,
`update_time` = CURRENT_TIMESTAMP;";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@storeName", storeName);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RecordRecommendationFeedbackAsync(int userId, RecommendationFeedbackRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"
INSERT INTO `food_question_form_test`
(`ID`, `taste`, `hunger`, `hot_cold`, `eat_mode`, `distance`, `price`, `type`, `agree`, `商家名稱`)
VALUES
(@userId, @taste, @hunger, @hotCold, @eatMode, @distance, @price, @type, @agree, @storeName);";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@taste", request.TasteId);
        command.Parameters.AddWithValue("@hunger", request.HungerId);
        command.Parameters.AddWithValue("@hotCold", request.HotColdId);
        command.Parameters.AddWithValue("@eatMode", request.EatModeId);
        command.Parameters.AddWithValue("@distance", request.DistanceId);
        command.Parameters.AddWithValue("@price", request.PriceId);
        command.Parameters.AddWithValue("@type", request.Type);
        command.Parameters.AddWithValue("@agree", request.Agree ? 1 : 0);
        command.Parameters.AddWithValue("@storeName", string.IsNullOrWhiteSpace(request.StoreName) ? DBNull.Value : request.StoreName);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task IncrementUserPreferenceByStoreAsync(int userId, string storeName, CancellationToken cancellationToken)
    {
        const string typeSql = "SELECT `c3` FROM `food_back` WHERE `商家名稱` = @storeName LIMIT 1;";

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        string? rawType;
        await using (var typeCmd = new MySqlCommand(typeSql, connection))
        {
            typeCmd.Parameters.AddWithValue("@storeName", storeName);
            rawType = Convert.ToString(await typeCmd.ExecuteScalarAsync(cancellationToken));
        }

        if (string.IsNullOrWhiteSpace(rawType))
        {
            return;
        }

        var column = NormalizeUserDataColumn(rawType);
        if (column is null)
        {
            return;
        }

        const string ensureSql = "INSERT INTO `user_data` (`ID`) VALUES (@userId) ON DUPLICATE KEY UPDATE `ID` = `ID`;";
        await using (var ensureCmd = new MySqlCommand(ensureSql, connection))
        {
            ensureCmd.Parameters.AddWithValue("@userId", userId);
            await ensureCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        var updateSql = $"UPDATE `user_data` SET `{column}` = `{column}` + 1 WHERE `ID` = @userId;";
        await using var updateCmd = new MySqlCommand(updateSql, connection);
        updateCmd.Parameters.AddWithValue("@userId", userId);
        await updateCmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string? NormalizeUserDataColumn(string rawType)
    {
        if (UserDataColumnAlias.TryGetValue(rawType, out var mapped))
        {
            return mapped;
        }

        if (UserDataColumns.Contains(rawType))
        {
            return rawType;
        }

        return null;
    }
}
