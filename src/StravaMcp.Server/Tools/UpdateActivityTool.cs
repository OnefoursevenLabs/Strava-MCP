using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;

namespace StravaMcp.Server.Tools;

public sealed class UpdateActivityTool : IStravaTool
{
    private readonly StravaClient _client;
    public UpdateActivityTool(StravaClient client) => _client = client;

    public ToolDefinition Definition => new()
    {
        Name = "update_activity",
        Description = "Update an activity's mutable properties (name, description, type, gear, etc.).",
        InputSchema = ToolHelpers.Schema(
            ("activity_id", "string", "The activity's numeric ID.", true),
            ("name", "string", "New name for the activity.", false),
            ("description", "string", "New description.", false),
            ("sport_type", "string", "Sport type (e.g. Run, Ride, Swim, Hike, Walk).", false),
            ("gear_id", "string", "Gear ID to associate, or 'none' to remove.", false),
            ("commute", "string", "Set to 'true' or 'false'.", false),
            ("trainer", "string", "Set to 'true' or 'false' for indoor trainer.", false)
        )
    };

    public async Task<ToolCallResult> ExecuteAsync(JsonObject args)
    {
        var id = ToolHelpers.GetString(args, "activity_id")
            ?? throw new ArgumentException("activity_id is required");

        var payload = new JsonObject();
        if (ToolHelpers.GetString(args, "name") is { } name) payload["name"] = name;
        if (ToolHelpers.GetString(args, "description") is { } desc) payload["description"] = desc;
        if (ToolHelpers.GetString(args, "sport_type") is { } st) payload["sport_type"] = st;
        if (ToolHelpers.GetString(args, "gear_id") is { } gear) payload["gear_id"] = gear;
        if (ToolHelpers.GetString(args, "commute") is { } commute) payload["commute"] = commute == "true";
        if (ToolHelpers.GetString(args, "trainer") is { } trainer) payload["trainer"] = trainer == "true";

        var data = await _client.PutAsync($"/activities/{id}", payload);
        return ToolHelpers.Success(data);
    }
}
