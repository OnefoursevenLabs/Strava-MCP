using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace StravaMcp.Server.Strava;

/// <summary>
/// HTTP client for the Strava v3 API with automatic OAuth2 token refresh.
/// </summary>
public sealed class StravaClient : IDisposable
{
    private const string BaseUrl = "https://api.strava.com/api/v3";
    private const string TokenUrl = "https://www.strava.com/oauth/token";

    private readonly HttpClient _http = new();
    private readonly StravaConfig _config;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private TokenData _token;

    public StravaClient(StravaConfig config)
    {
        _config = config;
        _token = LoadTokenFromDisk() ?? new TokenData { RefreshToken = config.RefreshToken };
    }

    public async Task<JsonNode?> GetAsync(string path, Dictionary<string, string>? queryParams = null)
    {
        await EnsureValidTokenAsync();

        var url = $"{BaseUrl}{path}";
        if (queryParams is { Count: > 0 })
        {
            var qs = string.Join("&", queryParams.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            url = $"{url}?{qs}";
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

        var response = await _http.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Force token refresh and retry once
            await RefreshTokenAsync();
            using var retry = new HttpRequestMessage(HttpMethod.Get, url);
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            response = await _http.SendAsync(retry);
        }

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new StravaApiException((int)response.StatusCode, body);

        return JsonNode.Parse(body);
    }

    public async Task<JsonNode?> PutAsync(string path, JsonObject? payload = null)
    {
        await EnsureValidTokenAsync();

        var url = $"{BaseUrl}{path}";
        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
        if (payload is not null)
            request.Content = new StringContent(payload.ToJsonString(), System.Text.Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new StravaApiException((int)response.StatusCode, body);

        return JsonNode.Parse(body);
    }

    private async Task EnsureValidTokenAsync()
    {
        if (!_token.IsExpired && !string.IsNullOrEmpty(_token.AccessToken))
            return;

        await RefreshTokenAsync();
    }

    private async Task RefreshTokenAsync()
    {
        await _tokenLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (!_token.IsExpired && !string.IsNullOrEmpty(_token.AccessToken))
                return;

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _config.ClientId,
                ["client_secret"] = _config.ClientSecret,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _token.RefreshToken
            });

            var response = await _http.PostAsync(TokenUrl, form);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new StravaApiException((int)response.StatusCode, $"Token refresh failed: {body}");

            var refreshed = JsonSerializer.Deserialize<TokenData>(body)
                ?? throw new InvalidOperationException("Failed to deserialize token response");

            _token = refreshed;
            SaveTokenToDisk(_token);
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private TokenData? LoadTokenFromDisk()
    {
        try
        {
            if (File.Exists(_config.TokenFilePath))
            {
                var json = File.ReadAllText(_config.TokenFilePath);
                return JsonSerializer.Deserialize<TokenData>(json);
            }
        }
        catch
        {
            // Fall through – will use config refresh token
        }
        return null;
    }

    private void SaveTokenToDisk(TokenData token)
    {
        try
        {
            var json = JsonSerializer.Serialize(token, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_config.TokenFilePath, json);
        }
        catch
        {
            // Non-fatal – token still lives in memory
        }
    }

    public void Dispose()
    {
        _http.Dispose();
        _tokenLock.Dispose();
    }
}
