using System.Text.Json;
using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;

namespace StravaMcp.Server.Tools;

internal static class ToolHelpers
{
    private static readonly JsonSerializerOptions PrettyJson = new() { WriteIndented = true };

    public static ToolCallResult Success(JsonNode? data)
    {
        var text = data is not null
            ? data.ToJsonString(PrettyJson)
            : "No data returned";

        return new ToolCallResult { Content = [new ContentBlock { Text = text }] };
    }

    public static ToolCallResult Success(string message)
    {
        return new ToolCallResult { Content = [new ContentBlock { Text = message }] };
    }

    public static string? GetString(JsonObject args, string key)
        => args.TryGetPropertyValue(key, out var val) ? val?.GetValue<string>() : null;

    public static int? GetInt(JsonObject args, string key)
        => args.TryGetPropertyValue(key, out var val) && val is not null ? val.GetValue<int>() : null;

    public static bool? GetBool(JsonObject args, string key)
        => args.TryGetPropertyValue(key, out var val) && val is not null ? val.GetValue<bool>() : null;

    public static JsonObject Schema(params (string name, string type, string description, bool required)[] props)
    {
        var properties = new JsonObject();
        var required = new JsonArray();

        foreach (var (name, type, description, isRequired) in props)
        {
            properties[name] = new JsonObject
            {
                ["type"] = type,
                ["description"] = description
            };
            if (isRequired)
                required.Add(name);
        }

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required
        };
    }
}
