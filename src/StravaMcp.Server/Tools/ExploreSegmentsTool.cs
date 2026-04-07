using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class ExploreSegmentsTool : IStravaTool
{
    private readonly StravaClient _client;
    public ExploreSegmentsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "explore_segments",
        Description = "Find popular segments within a geographic bounding box. Returns up to 10 segments.",
        InputSchema = ToolHelpers.Schema(
            ("south_west_lat", "string", "Southwest corner latitude.", true),
            ("south_west_lng", "string", "Southwest corner longitude.", true),
            ("north_east_lat", "string", "Northeast corner latitude.", true),
            ("north_east_lng", "string", "Northeast corner longitude.", true),
            ("activity_type", "string", "Filter by 'running' or 'riding' (default riding).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var sw_lat = ToolHelpers.GetString(args, "south_west_lat") ?? throw new ArgumentException("south_west_lat required");
        var sw_lng = ToolHelpers.GetString(args, "south_west_lng") ?? throw new ArgumentException("south_west_lng required");
        var ne_lat = ToolHelpers.GetString(args, "north_east_lat") ?? throw new ArgumentException("north_east_lat required");
        var ne_lng = ToolHelpers.GetString(args, "north_east_lng") ?? throw new ArgumentException("north_east_lng required");

        var qp = new Dictionary<string, string>
        {
            ["bounds"] = $"{sw_lat},{sw_lng},{ne_lat},{ne_lng}"
        };
        if (ToolHelpers.GetString(args, "activity_type") is { } at) qp["activity_type"] = at;

        var data = await _client.GetAsync("/segments/explore", qp);
        return ToolHelpers.Success(data);
    }
}
