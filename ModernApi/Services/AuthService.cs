using FoodieCare.ModernApi.Data;
using FoodieCare.ModernApi.Models;

namespace FoodieCare.ModernApi.Services;

public sealed class AuthService
{
    private readonly IRecommendationRepository _repository;
    private readonly AuthTokenService _tokenService;

    public AuthService(IRecommendationRepository repository, AuthTokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _repository.ValidateUserAsync(request.Username, request.Password, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return _tokenService.CreateToken(new UserIdentity
        {
            UserId = user.Value.UserId,
            Username = user.Value.Username
        });
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetUserByUsernameAsync(request.Username, cancellationToken);
        if (exists is not null)
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var created = await _repository.CreateUserAsync(request.Username, request.Password, request.Gender, request.Age, cancellationToken);

        return _tokenService.CreateToken(new UserIdentity
        {
            UserId = created.UserId,
            Username = created.Username
        });
    }

    public bool TryValidateToken(string? token, out UserIdentity? identity)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            identity = null;
            return false;
        }

        return _tokenService.TryValidate(token, out identity);
    }
}
