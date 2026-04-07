using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetActivityLapsTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetActivityLapsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_activity_laps",
        Description = "Get the laps of a specific activity.",
        InputSchema = ToolHelpers.Schema(
            ("activity_id", "string", "The activity's numeric ID.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "activity_id")
            ?? throw new ArgumentException("activity_id is required");
        var data = await _client.GetAsync($"/activities/{id}/laps");
        return ToolHelpers.Success(data);
    }
}
