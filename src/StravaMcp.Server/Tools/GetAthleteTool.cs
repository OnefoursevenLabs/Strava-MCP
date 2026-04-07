using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetAthleteTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetAthleteTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_athlete",
        Description = "Get the profile of the currently authenticated Strava athlete, including name, stats preferences, and club memberships.",
        InputSchema = ToolHelpers.Schema()
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var data = await _client.GetAsync("/athlete");
        return ToolHelpers.Success(data);
    }
}
