using System.Security.Cryptography;
using System.Text;
using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Options;

namespace FoodieCare.ModernApi.Services;

public sealed class AuthTokenService
{
    private readonly byte[] _secret;
    private readonly int _tokenHours;

    public AuthTokenService(IOptions<FoodieCareOptions> options)
    {
        var secret = options.Value.AuthSecret;
        _secret = Encoding.UTF8.GetBytes(secret);
        _tokenHours = Math.Max(1, options.Value.AuthTokenHours);
    }

    public AuthResponse CreateToken(UserIdentity user)
    {
        var expires = DateTimeOffset.UtcNow.AddHours(_tokenHours);
        var payload = $"{user.UserId}|{expires.ToUnixTimeSeconds()}|{user.Username}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var payloadB64 = Base64UrlEncode(payloadBytes);

        using var hmac = new HMACSHA256(_secret);
        var sig = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadB64));
        var token = $"{payloadB64}.{Base64UrlEncode(sig)}";

        return new AuthResponse
        {
            UserId = user.UserId,
            Username = user.Username,
            Token = token,
            ExpiresAtUtc = expires
        };
    }

    public bool TryValidate(string token, out UserIdentity? user)
    {
        user = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parts = token.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        var payloadB64 = parts[0];
        var signatureB64 = parts[1];

        byte[] actualSig;
        try
        {
            actualSig = Base64UrlDecode(signatureB64);
        }
        catch
        {
            return false;
        }

        using var hmac = new HMACSHA256(_secret);
        var expectedSig = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadB64));
        if (!CryptographicOperations.FixedTimeEquals(actualSig, expectedSig))
        {
            return false;
        }

        string payload;
        try
        {
            payload = Encoding.UTF8.GetString(Base64UrlDecode(payloadB64));
        }
        catch
        {
            return false;
        }

        var tokens = payload.Split('|');
        if (tokens.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(tokens[0], out var userId))
        {
            return false;
        }

        if (!long.TryParse(tokens[1], out var expiresUnix))
        {
            return false;
        }

        if (DateTimeOffset.UtcNow > DateTimeOffset.FromUnixTimeSeconds(expiresUnix))
        {
            return false;
        }

        user = new UserIdentity
        {
            UserId = userId,
            Username = tokens[2]
        };

        return true;
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }

        return Convert.FromBase64String(padded);
    }
}
