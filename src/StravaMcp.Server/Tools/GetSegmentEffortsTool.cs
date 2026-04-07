using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class GetSegmentEffortsTool : IStravaTool
{
    private readonly StravaClient _client;
    public GetSegmentEffortsTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "get_segment_efforts",
        Description = "List the authenticated athlete's efforts on a given segment. Optionally filter by date range.",
        InputSchema = ToolHelpers.Schema(
            ("segment_id", "string", "The segment's numeric ID.", true),
            ("start_date_local", "string", "ISO 8601 start date filter.", false),
            ("end_date_local", "string", "ISO 8601 end date filter.", false),
            ("per_page", "integer", "Items per page (default 30).", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "segment_id")
            ?? throw new ArgumentException("segment_id is required");

        var qp = new Dictionary<string, string> { ["segment_id"] = id };
        if (ToolHelpers.GetString(args, "start_date_local") is { } start) qp["start_date_local"] = start;
        if (ToolHelpers.GetString(args, "end_date_local") is { } end) qp["end_date_local"] = end;
        if (ToolHelpers.GetInt(args, "per_page") is { } pp) qp["per_page"] = pp.ToString();

        var data = await _client.GetAsync("/segment_efforts", qp);
        return ToolHelpers.Success(data);
    }
}
