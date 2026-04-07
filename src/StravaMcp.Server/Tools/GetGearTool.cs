using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetGearTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetGearTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_gear",
        Description = "Get details about a specific piece of gear (shoes, bike, etc.) by ID.",
        InputSchema = ToolHelpers.Schema(
            ("gear_id", "string", "The gear ID (e.g. 'b12345' for bikes, 'g12345' for shoes).", true)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "gear_id")
            ?? throw new ArgumentException("gear_id is required");
        var data = await _client.GetAsync($"/gear/{id}");
        return ToolHelpers.Success(data);
    }
}
