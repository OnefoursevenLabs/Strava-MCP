using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class ListClubsTool : IStravaTool
{
    private readonly StravaClient _client;
    public ListClubsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "list_clubs",
        Description = "List clubs the authenticated athlete is a member of.",
        InputSchema = ToolHelpers.Schema(
            ("page", "integer", "Page number (default 1).", false),
            ("per_page", "integer", "Items per page (default 30).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetInt(args, "page") is { } page) qp["page"] = page.ToString();
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();

        var data = await _client.GetAsync("/athlete/clubs", qp);
        return ToolHelpers.Success(data);
    }
}
