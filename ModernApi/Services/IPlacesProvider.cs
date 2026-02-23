using FoodieCare.ModernApi.Models;

namespace FoodieCare.ModernApi.Services;

public interface IPlacesProvider
{
    string Name { get; }

    Task<IReadOnlyList<HybridPlaceDto>> SearchNearbyAsync(
        HybridPlacesSearchRequest request,
        CancellationToken cancellationToken);
}
