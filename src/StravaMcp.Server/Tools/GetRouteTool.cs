using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetRouteTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetRouteTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_route",
        Description = "Get detailed information about a specific route by ID.",
        InputSchema = ToolHelpers.Schema(
            ("route_id", "string", "The route's numeric ID.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "route_id")
            ?? throw new ArgumentException("route_id is required");
        var data = await _client.GetAsync($"/routes/{id}");
        return ToolHelpers.Success(data);
    }
}
