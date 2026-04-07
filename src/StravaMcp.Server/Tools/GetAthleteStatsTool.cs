using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetAthleteStatsTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetAthleteStatsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_athlete_stats",
        Description = "Get aggregated statistics for the authenticated athlete (totals and recent for run, ride, swim). Requires the athlete ID.",
        InputSchema = ToolHelpers.Schema(
            ("athlete_id", "string", "The athlete's numeric ID. Use get_athlete to find it.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "athlete_id")
            ?? throw new ArgumentException("athlete_id is required");
        var data = await _client.GetAsync($"/athletes/{id}/stats");
        return ToolHelpers.Success(data);
    }
}
