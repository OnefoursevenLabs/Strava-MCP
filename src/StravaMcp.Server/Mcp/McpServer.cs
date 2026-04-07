using System.Text.Json;
using System.Text.Json.Nodes;
using StravaMcp.Server.Tools;

namespace StravaMcp.Server.Mcp;

/// <summary>
/// MCP server that communicates via JSON-RPC over stdin/stdout.
/// </summary>
public sealed class McpServer
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IReadOnlyList<IStravaTool> _tools;

    public McpServer(IReadOnlyList<IStravaTool> tools)
    {
        _tools = tools;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        using var stdin = Console.OpenStandardInput();
        using var reader = new StreamReader(stdin);

        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null)
                break; // EOF

            if (string.IsNullOrWhiteSpace(line))
                continue;

            JsonRpcRequest? request;
            try
            {
                request = JsonSerializer.Deserialize<JsonRpcRequest>(line, JsonOpts);
            }
            catch
            {
                WriteResponse(new JsonRpcResponse
                {
                    Error = new JsonRpcError { Code = -32700, Message = "Parse error" }
                });
                continue;
            }

            if (request is null)
                continue;

            var response = await HandleRequestAsync(request);
            if (response is not null)
                WriteResponse(response);
        }
    }

    private async Task<JsonRpcResponse?> HandleRequestAsync(JsonRpcRequest request)
    {
        return request.Method switch
        {
            "initialize" => HandleInitialize(request),
            "notifications/initialized" => null, // notification – no response
            "tools/list" => HandleToolsList(request),
            "tools/call" => await HandleToolCallAsync(request),
            "ping" => new JsonRpcResponse { Id = request.Id, Result = new JsonObject() },
            _ => new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError { Code = -32601, Message = $"Method not found: {request.Method}" }
            }
        };
    }

    private JsonRpcResponse HandleInitialize(JsonRpcRequest request)
    {
        var result = new JsonObject
        {
            ["protocolVersion"] = "2024-11-05",
            ["capabilities"] = new JsonObject
            {
                ["tools"] = new JsonObject { ["listChanged"] = false }
            },
            ["serverInfo"] = new JsonObject
            {
                ["name"] = "strava-mcp-server",
                ["version"] = "1.0.0"
            }
        };

        return new JsonRpcResponse { Id = request.Id, Result = result };
    }

    private JsonRpcResponse HandleToolsList(JsonRpcRequest request)
    {
        var toolDefs = new JsonArray();
        foreach (var tool in _tools)
        {
            var def = tool.Definition;
            toolDefs.Add(JsonSerializer.SerializeToNode(def, JsonOpts));
        }

        var result = new JsonObject { ["tools"] = toolDefs };
        return new JsonRpcResponse { Id = request.Id, Result = result };
    }

    private async Task<JsonRpcResponse> HandleToolCallAsync(JsonRpcRequest request)
    {
        var toolName = request.Params?["name"]?.GetValue<string>();
        var args = request.Params?["arguments"]?.AsObject();

        if (string.IsNullOrEmpty(toolName))
        {
            return new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError { Code = -32602, Message = "Missing tool name" }
            };
        }

        var tool = _tools.FirstOrDefault(t => t.Definition.Name == toolName);
        if (tool is null)
        {
            return new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError { Code = -32602, Message = $"Unknown tool: {toolName}" }
            };
        }

        try
        {
            var result = await tool.ExecuteAsync(args ?? new JsonObject());
            return new JsonRpcResponse
            {
                Id = request.Id,
                Result = JsonSerializer.SerializeToNode(result, JsonOpts)
            };
        }
        catch (Exception ex)
        {
            var errorResult = new ToolCallResult
            {
                Content = [new ContentBlock { Text = $"Error: {ex.Message}" }],
                IsError = true
            };
            return new JsonRpcResponse
            {
                Id = request.Id,
                Result = JsonSerializer.SerializeToNode(errorResult, JsonOpts)
            };
        }
    }

    private static void WriteResponse(JsonRpcResponse response)
    {
        var json = JsonSerializer.Serialize(response, JsonOpts);
        Console.Out.WriteLine(json);
        Console.Out.Flush();
    }
}
