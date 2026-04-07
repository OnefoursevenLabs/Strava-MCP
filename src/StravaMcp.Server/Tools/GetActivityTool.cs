using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetActivityTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetActivityTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_activity",
        Description = "Get full details for a specific activity by ID, including splits, laps, segment efforts, and gear.",
        InputSchema = ToolHelpers.Schema(
            ("activity_id", "string", "The activity's numeric ID.", true),
            ("include_all_efforts", "string", "Set to 'true' to include all segment efforts (default false).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "activity_id")
            ?? throw new ArgumentException("activity_id is required");

        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetString(args, "include_all_efforts") == "true")
            qp["include_all_efforts"] = "true";

        var data = await _client.GetAsync($"/activities/{id}", qp);
        return ToolHelpers.Success(data);
    }
}
