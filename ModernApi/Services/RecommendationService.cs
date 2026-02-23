using FoodieCare.ModernApi.Data;
using FoodieCare.ModernApi.Domain;
using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace FoodieCare.ModernApi.Services;

public sealed class RecommendationService
{
    private readonly RuleBasedTypeRecommender _typeRecommender;
    private readonly IRecommendationRepository _repository;
    private readonly FoodieCareOptions _options;

    public RecommendationService(
        RuleBasedTypeRecommender typeRecommender,
        IRecommendationRepository repository,
        IOptions<FoodieCareOptions> options)
    {
        _typeRecommender = typeRecommender;
        _repository = repository;
        _options = options.Value;
    }

    public async Task<RecommendOptionsResponse> GetRecommendationOptionsAsync(RecommendRequest request, CancellationToken cancellationToken)
    {
        var profile = new PreferenceProfile(
            request.Price,
            request.EatMode,
            request.Hunger,
            request.Distance,
            request.HotCold,
            request.Taste);

        var primaryType = _typeRecommender.Recommend(profile);

        var candidates = new List<string> { primaryType };

        try
        {
            var association = await _repository.GetAssociationCandidatesAsync(primaryType, _options.DefaultCandidateCount, cancellationToken);
            AppendDistinct(candidates, association, _options.DefaultCandidateCount);
        }
        catch (MySqlException)
        {
        }

        if (request.UserId is int userId)
        {
            try
            {
                var userRecent = await _repository.GetRecentUserTypesAsync(userId, _options.DefaultCandidateCount, cancellationToken);
                AppendDistinct(candidates, userRecent, _options.DefaultCandidateCount);
            }
            catch (MySqlException)
            {
            }
        }

        var nearbyDistanceKm = ParseDistanceKm(request.Distance, _options.DefaultDistanceKm);
        try
        {
            var nearbyTypes = await _repository.GetNearbyTypesAsync(
                request.Latitude,
                request.Longitude,
                nearbyDistanceKm,
                _options.DefaultCandidateCount + 2,
                cancellationToken);

            if (nearbyTypes.Count > 0)
            {
                var fallbackPrimary = nearbyTypes[0];
                if (!nearbyTypes.Contains(primaryType, StringComparer.Ordinal))
                {
                    primaryType = fallbackPrimary;
                }

                var reordered = new List<string> { primaryType };
                AppendDistinct(reordered, nearbyTypes, _options.DefaultCandidateCount);
                AppendDistinct(reordered, candidates, _options.DefaultCandidateCount);
                candidates = reordered;
            }
        }
        catch (MySqlException)
        {
        }

        return new RecommendOptionsResponse
        {
            PrimaryType = primaryType,
            CandidateTypes = candidates
        };
    }

    public Task<IReadOnlyList<StoreDto>> SearchStoresAsync(StoreSearchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
        {
            throw new ArgumentException("Type is required.", nameof(request));
        }

        var distance = request.DistanceKm.GetValueOrDefault(_options.DefaultDistanceKm);
        var limit = request.Limit.GetValueOrDefault(20);
        var band = PriceBandParser.Parse(request.Price);

        return _repository.SearchStoresAsync(
            request.Type,
            band,
            request.Latitude,
            request.Longitude,
            distance,
            limit,
            cancellationToken);
    }

    private static void AppendDistinct(List<string> target, IReadOnlyList<string> source, int maxCount)
    {
        foreach (var item in source)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            if (target.Contains(item, StringComparer.Ordinal))
            {
                continue;
            }

            target.Add(item);
            if (target.Count >= maxCount)
            {
                break;
            }
        }
    }

    private static int ParseDistanceKm(string? value, int fallback)
    {
        return value switch
        {
            "1km_less" => 1,
            "1to5km" => 5,
            "5to10km" => 10,
            "10km_more" => 20,
            _ => fallback
        };
    }
}
