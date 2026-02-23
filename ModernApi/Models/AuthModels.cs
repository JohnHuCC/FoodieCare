namespace FoodieCare.ModernApi.Models;

public sealed class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public int Gender { get; set; }
    public int Age { get; set; }
}

public sealed class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public sealed class AuthResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Token { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
}

public sealed class UserIdentity
{
    public int UserId { get; set; }
    public required string Username { get; set; }
}
