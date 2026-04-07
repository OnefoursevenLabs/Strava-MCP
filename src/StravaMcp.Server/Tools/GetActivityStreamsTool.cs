using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetActivityStreamsTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetActivityStreamsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_activity_streams",
        Description = "Get time-series data streams for an activity (GPS, heart rate, power, cadence, altitude, etc.).",
        InputSchema = ToolHelpers.Schema(
            ("activity_id", "string", "The activity's numeric ID.", true),
            ("stream_types", "string", "Comma-separated stream types: time, distance, latlng, altitude, heartrate, cadence, watts, temp, moving, grade_smooth, velocity_smooth.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "activity_id")
            ?? throw new ArgumentException("activity_id is required");
        var types = ToolHelpers.GetString(args, "stream_types")
            ?? throw new ArgumentException("stream_types is required");

        var qp = new Dictionary<string, string>
        {
            ["keys"] = types,
            ["key_by_type"] = "true"
        };

        var data = await _client.GetAsync($"/activities/{id}/streams", qp);
        return ToolHelpers.Success(data);
    }
}
