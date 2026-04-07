using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetActivityKudosTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetActivityKudosTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_activity_kudos",
        Description = "Get the list of athletes who gave kudos on a specific activity.",
        InputSchema = ToolHelpers.Schema(
            ("activity_id", "string", "The activity's numeric ID.", true),
            ("page", "integer", "Page number (default 1).", false),
            ("per_page", "integer", "Items per page (default 30).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "activity_id")
            ?? throw new ArgumentException("activity_id is required");
        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetInt(args, "page") is { } page) qp["page"] = page.ToString();
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();

        var data = await _client.GetAsync($"/activities/{id}/kudos", qp);
        return ToolHelpers.Success(data);
    }
}
