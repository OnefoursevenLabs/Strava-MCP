namespace StravaMcp.Server.Strava;

public sealed class StravaConfig
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RefreshToken { get; init; }
    public string TokenFilePath { get; init; } = "strava_tokens.json";
}
