using System.Text.Json.Nodes;
using StravaMcp.Server.Mcp;

namespace StravaMcp.Server.Tools;

public interface IStravaTool
{
    ToolDefinition Definition { get; }
    Task<ToolCallResult> ExecuteAsync(JsonObject args);
}
