using FoodieCare.ModernApi.Data;
using FoodieCare.ModernApi.Domain;
using FoodieCare.ModernApi.Models;
using FoodieCare.ModernApi.Options;
using FoodieCare.ModernApi.Services;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Globalization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FoodieCareOptions>(builder.Configuration.GetSection(FoodieCareOptions.SectionName));
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("overpass", client =>
{
    client.BaseAddress = new Uri("https://overpass-api.de/api/");
    client.Timeout = TimeSpan.FromSeconds(20);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("FoodieCare/1.0 (+https://github.com/JohnHuCC/FoodieCare)");
});
builder.Services.AddHttpClient("google-places", client =>
{
    client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/place/");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<RuleBasedTypeRecommender>();
builder.Services.AddSingleton<AuthTokenService>();
builder.Services.AddScoped<IRecommendationRepository, MySqlRecommendationRepository>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InteractionService>();
builder.Services.AddScoped<IPlacesProvider, OsmPlacesProvider>();
builder.Services.AddScoped<IPlacesProvider, GooglePlacesProvider>();
builder.Services.AddScoped<HybridPlacesService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (MySqlException ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            message = $"Database error: {ex.Message}"
        }));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            message = $"Server error: {ex.Message}"
        }));
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    service = "FoodieCare.ModernApi",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/api/seed/count", (IRecommendationRepository repository) =>
{
    if (repository is MySqlRecommendationRepository mySqlRepository)
    {
        var count = mySqlRepository.DebugGetSeedCount();
        var profile = mySqlRepository.DebugGetActiveRankingProfile();
        return Results.Ok(new { seedCount = count, activeRankingProfile = profile });
    }

    return Results.Ok(new { seedCount = 0, activeRankingProfile = "n/a" });
});

app.MapGet("/api/geocode", async (string query, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.BadRequest("query is required.");
    }

    using var http = new HttpClient();
    http.DefaultRequestHeaders.UserAgent.ParseAdd("FoodieCare/1.0 (+https://github.com/JohnHuCC/FoodieCare)");
    http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

    // Primary provider: OpenStreetMap Nominatim
    var nominatimUrl = $"https://nominatim.openstreetmap.org/search?format=json&limit=1&q={Uri.EscapeDataString(query)}";
    try
    {
        var response = await http.GetAsync(nominatimUrl, ct);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var first = doc.RootElement[0];
                if (first.TryGetProperty("lat", out var latEl) &&
                    first.TryGetProperty("lon", out var lonEl) &&
                    double.TryParse(latEl.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(lonEl.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lng))
                {
                    var name = first.TryGetProperty("display_name", out var nameEl) ? nameEl.GetString() : query;
                    return Results.Ok(new { latitude = lat, longitude = lng, provider = "nominatim", displayName = name });
                }
            }
        }
    }
    catch
    {
    }

    // Fallback provider: Open-Meteo geocoding
    var meteoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(query)}&count=1&language=en&format=json";
    try
    {
        var response = await http.GetAsync(meteoUrl, ct);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("results", out var results) &&
                results.ValueKind == JsonValueKind.Array &&
                results.GetArrayLength() > 0)
            {
                var first = results[0];
                if (first.TryGetProperty("latitude", out var latEl) &&
                    first.TryGetProperty("longitude", out var lngEl) &&
                    latEl.TryGetDouble(out var lat) &&
                    lngEl.TryGetDouble(out var lng))
                {
                    var name = first.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : query;
                    return Results.Ok(new { latitude = lat, longitude = lng, provider = "open-meteo", displayName = name });
                }
            }
        }
    }
    catch
    {
    }

    return Results.NotFound(new { message = $"No geocoding result for '{query}'." });
});

app.MapPost("/api/places/hybrid-search", async (HybridPlacesSearchRequest request, HybridPlacesService service, IOptions<FoodieCareOptions> options, CancellationToken ct) =>
{
    if (!options.Value.HybridPlaces.Enabled)
    {
        return Results.BadRequest("Hybrid places search is disabled.");
    }

    if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
    {
        return Results.BadRequest("Invalid coordinates.");
    }

    var result = await service.SearchAsync(request, ct);
    return Results.Ok(result);
});

app.MapPost("/api/auth/register", async (RegisterRequest request, AuthService authService, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest("Username and password are required.");
    }

    try
    {
        var result = await authService.RegisterAsync(request, ct);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapPost("/api/auth/login", async (LoginRequest request, AuthService authService, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest("Username and password are required.");
    }

    var result = await authService.LoginAsync(request, ct);
    return result is null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapPost("/api/recommendation/options", async (RecommendRequest request, RecommendationService service, CancellationToken ct) =>
{
    if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
    {
        return Results.BadRequest("Invalid coordinates.");
    }

    var options = await service.GetRecommendationOptionsAsync(request, ct);
    return Results.Ok(options);
});

app.MapPost("/api/recommendation/stores", async (StoreSearchRequest request, RecommendationService service, CancellationToken ct) =>
{
    if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
    {
        return Results.BadRequest("Invalid coordinates.");
    }

    if (string.IsNullOrWhiteSpace(request.Type))
    {
        return Results.BadRequest("Type is required.");
    }

    var stores = await service.SearchStoresAsync(request, ct);
    return Results.Ok(stores);
});

app.MapPost("/api/stores/browse", async (StoreSearchRequest request, IRecommendationRepository repository, IOptions<FoodieCareOptions> options, CancellationToken ct) =>
{
    if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
    {
        return Results.BadRequest("Invalid coordinates.");
    }

    var distance = request.DistanceKm.GetValueOrDefault(options.Value.DefaultDistanceKm);
    var limit = request.Limit.GetValueOrDefault(50);
    var price = PriceBandParser.Parse(request.Price);
    var stores = await repository.BrowseStoresAsync(
        request.Type,
        price,
        request.Latitude,
        request.Longitude,
        distance,
        limit,
        ct);

    return Results.Ok(stores);
});

app.MapPost("/api/interactions/click", async (HttpContext http, RecordClickRequest request, AuthService authService, InteractionService interactionService, CancellationToken ct) =>
{
    if (!TryGetCurrentUser(http, authService, out var user))
    {
        return Results.Unauthorized();
    }

    if (string.IsNullOrWhiteSpace(request.StoreName))
    {
        return Results.BadRequest("StoreName is required.");
    }

    await interactionService.RecordClickAsync(user!.UserId, request.StoreName, ct);
    return Results.Ok(new { status = "recorded" });
});

app.MapPost("/api/interactions/recommendation-feedback", async (HttpContext http, RecommendationFeedbackRequest request, AuthService authService, InteractionService interactionService, CancellationToken ct) =>
{
    if (!TryGetCurrentUser(http, authService, out var user))
    {
        return Results.Unauthorized();
    }

    if (string.IsNullOrWhiteSpace(request.Type))
    {
        return Results.BadRequest("Type is required.");
    }

    await interactionService.RecordRecommendationFeedbackAsync(user!.UserId, request, ct);
    return Results.Ok(new { status = "recorded" });
});

app.Run();

static bool TryGetCurrentUser(HttpContext http, AuthService authService, out UserIdentity? identity)
{
    identity = null;

    if (!http.Request.Headers.TryGetValue("Authorization", out var values))
    {
        return false;
    }

    var header = values.ToString();
    if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    var token = header["Bearer ".Length..].Trim();
    return authService.TryValidateToken(token, out identity);
}
