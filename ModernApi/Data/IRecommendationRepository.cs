using FoodieCare.ModernApi.Domain;
using FoodieCare.ModernApi.Models;

namespace FoodieCare.ModernApi.Data;

public interface IRecommendationRepository
{
    Task<IReadOnlyList<StoreDto>> BrowseStoresAsync(
        string? type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StoreDto>> SearchStoresAsync(
        string type,
        PriceBand priceBand,
        double latitude,
        double longitude,
        int maxDistanceKm,
        int limit,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> GetAssociationCandidatesAsync(
        string type,
        int limit,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> GetRecentUserTypesAsync(
        int userId,
        int limit,
        CancellationToken cancellationToken);

    Task<UserIdentity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<(int UserId, string Username)?> ValidateUserAsync(string username, string password, CancellationToken cancellationToken);

    Task<(int UserId, string Username)> CreateUserAsync(string username, string password, int gender, int age, CancellationToken cancellationToken);

    Task RecordUserClickAsync(int userId, string storeName, CancellationToken cancellationToken);

    Task RecordRecommendationFeedbackAsync(int userId, RecommendationFeedbackRequest request, CancellationToken cancellationToken);

    Task IncrementUserPreferenceByStoreAsync(int userId, string storeName, CancellationToken cancellationToken);
}
