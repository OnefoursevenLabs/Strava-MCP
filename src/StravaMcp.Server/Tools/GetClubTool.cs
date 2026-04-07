using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetClubTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetClubTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_club",
        Description = "Get detailed information about a specific club by ID.",
        InputSchema = ToolHelpers.Schema(
            ("club_id", "string", "The club's numeric ID.", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "club_id")
            ?? throw new ArgumentException("club_id is required");
        var data = await _client.GetAsync($"/clubs/{id}");
        return ToolHelpers.Success(data);
    }
}
