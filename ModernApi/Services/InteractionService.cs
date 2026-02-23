using FoodieCare.ModernApi.Data;
using FoodieCare.ModernApi.Models;

namespace FoodieCare.ModernApi.Services;

public sealed class InteractionService
{
    private readonly IRecommendationRepository _repository;

    public InteractionService(IRecommendationRepository repository)
    {
        _repository = repository;
    }

    public async Task RecordClickAsync(int userId, string storeName, CancellationToken cancellationToken)
    {
        await _repository.RecordUserClickAsync(userId, storeName, cancellationToken);
        await _repository.IncrementUserPreferenceByStoreAsync(userId, storeName, cancellationToken);
    }

    public Task RecordRecommendationFeedbackAsync(int userId, RecommendationFeedbackRequest request, CancellationToken cancellationToken)
    {
        return _repository.RecordRecommendationFeedbackAsync(userId, request, cancellationToken);
    }
}
