using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class ListActivitiesTool : IStravaTool
{
    private readonly StravaClient _client;
    public ListActivitiesTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "list_activities",
        Description = "List the authenticated athlete's activities. Supports pagination and optional time-range filtering via epoch timestamps.",
        InputSchema = ToolHelpers.Schema(
            ("page", "integer", "Page number (default 1).", false),
            ("per_page", "integer", "Items per page, max 200 (default 30).", false),
            ("after", "integer", "Only activities after this epoch timestamp.", false),
            ("before", "integer", "Only activities before this epoch timestamp.", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetInt(args, "page") is { } page) qp["page"] = page.ToString();
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();
        if (ToolHelpers.GetInt(args, "after") is { } after) qp["after"] = after.ToString();
        if (ToolHelpers.GetInt(args, "before") is { } before) qp["before"] = before.ToString();

        var data = await _client.GetAsync("/athlete/activities", qp);
        return ToolHelpers.Success(data);
    }
}
