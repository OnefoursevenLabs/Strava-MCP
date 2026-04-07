using System.Text.Json.Serialization;

namespace StravaMcp.Server.Strava;

public sealed class TokenData
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = "";

    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= ExpiresAt - 60;
}
