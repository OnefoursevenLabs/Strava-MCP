using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetClubMembersTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetClubMembersTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_club_members",
        Description = "List members of a specific club.",
        InputSchema = ToolHelpers.Schema(
            ("club_id", "string", "The club's numeric ID.", true),
            ("page", "integer", "Page number (default 1).", false),
            ("per_page", "integer", "Items per page (default 30).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "club_id")
            ?? throw new ArgumentException("club_id is required");
        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetInt(args, "page") is { } page) qp["page"] = page.ToString();
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();

        var data = await _client.GetAsync($"/clubs/{id}/members", qp);
        return ToolHelpers.Success(data);
    }
}
