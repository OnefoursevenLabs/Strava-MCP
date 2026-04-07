using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class ListRoutesTool : IStravaTool
{
    private readonly StravaClient _client;
    public ListRoutesTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "list_routes",
        Description = "List routes created by the authenticated athlete.",
        InputSchema = ToolHelpers.Schema(
            ("page", "integer", "Page number (default 1).", false),
            ("per_page", "integer", "Items per page (default 30).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        // Need athlete ID first — fetch from /athlete
        var athlete = await _client.GetAsync("/athlete");
        var athleteId = athlete?["id"]?.ToString()
            ?? throw new InvalidOperationException("Could not determine athlete ID");

        var qp = new Dictionary<string, string>();
        if (ToolHelpers.GetInt(args, "page") is { } page) qp["page"] = page.ToString();
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();

        var data = await _client.GetAsync($"/athletes/{athleteId}/routes", qp);
        return ToolHelpers.Success(data);
    }
}
