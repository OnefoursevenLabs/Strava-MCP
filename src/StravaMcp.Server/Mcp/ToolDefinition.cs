using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace StravaMcp.Server.Mcp;

public sealed class ToolDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("inputSchema")]
    public required JsonObject InputSchema { get; init; }
}

public sealed class ToolCallResult
{
    [JsonPropertyName("content")]
    public required List<ContentBlock> Content { get; init; }

    [JsonPropertyName("isError")]
    public bool IsError { get; init; }
}

public sealed class ContentBlock
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; init; } = "";
}
