using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetSegmentTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetSegmentTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_segment",
        Description = "Get detailed information about a specific Strava segment by ID.",
        InputSchema = ToolHelpers.Schema(
            ("segment_id", "string", "The segment's numeric ID.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "segment_id")
            ?? throw new ArgumentException("segment_id is required");
        var data = await _client.GetAsync($"/segments/{id}");
        return ToolHelpers.Success(data);
    }
}
