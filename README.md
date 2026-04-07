# Strava MCP Server

A [Model Context Protocol](https://modelcontextprotocol.io/) (MCP) server that exposes the Strava API as tools for LLM assistants. Built with C# / .NET 8.

## Features

- **OAuth2 token management** with automatic refresh and persistent token storage
- **21 tools** covering the full Strava read API plus activity updates
- **Stdio transport** — works with Claude Desktop, Claude Code, and any MCP-compatible client

### Available Tools

| Tool | Description |
|------|-------------|
| `get_athlete` | Get authenticated athlete profile |
| `get_athlete_stats` | Get aggregated athlete statistics (run/ride/swim totals) |
| `list_activities` | List activities with pagination and time-range filtering |
| `get_activity` | Get full activity details (splits, laps, segments, gear) |
| `get_activity_laps` | Get laps for an activity |
| `get_activity_zones` | Get HR/power zone distribution for an activity |
| `get_activity_comments` | Get comments on an activity |
| `get_activity_kudos` | Get kudos givers for an activity |
| `get_activity_streams` | Get time-series data (GPS, HR, power, cadence, altitude) |
| `update_activity` | Update activity name, description, sport type, gear, etc. |
| `get_segment` | Get segment details |
| `get_segment_efforts` | List athlete's efforts on a segment |
| `explore_segments` | Find popular segments in a geographic bounding box |
| `list_starred_segments` | List athlete's starred segments |
| `list_routes` | List athlete's routes |
| `get_route` | Get route details |
| `get_gear` | Get gear details (bikes, shoes) |
| `list_clubs` | List athlete's clubs |
| `get_club` | Get club details |
| `get_club_activities` | List recent club member activities |
| `get_club_members` | List club members |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A Strava API application ([create one here](https://www.strava.com/settings/api))
- A valid refresh token with `read_all,activity:read_all,activity:write` scopes

### Getting a Refresh Token

1. Create an app at https://www.strava.com/settings/api
2. Use the OAuth2 authorization flow to get a refresh token with the required scopes. You can use the [Strava OAuth2 playground](https://developers.strava.com/playground/) or a manual flow:

```
https://www.strava.com/oauth/authorize?client_id=YOUR_CLIENT_ID&response_type=code&redirect_uri=http://localhost&scope=read_all,activity:read_all,activity:write&approval_prompt=force
```

3. Exchange the authorization code for tokens:

```bash
curl -X POST https://www.strava.com/oauth/token \
  -d client_id=YOUR_CLIENT_ID \
  -d client_secret=YOUR_CLIENT_SECRET \
  -d code=AUTHORIZATION_CODE \
  -d grant_type=authorization_code
```

## Installation

### As a .NET global tool (recommended)

```bash
dotnet tool install --global StravaMcp.Server
```

Then run it anywhere with:

```bash
strava-mcp-server
```

### From source

```bash
cd src/StravaMcp.Server
dotnet build
```

Set environment variables:

```bash
export STRAVA_CLIENT_ID="your_client_id"
export STRAVA_CLIENT_SECRET="your_client_secret"
export STRAVA_REFRESH_TOKEN="your_refresh_token"
# Optional: custom path for cached tokens
export STRAVA_TOKEN_FILE="/path/to/strava_tokens.json"
```

## Usage

### With Claude Desktop

If installed as a global tool:

```json
{
  "mcpServers": {
    "strava": {
      "command": "strava-mcp-server",
      "env": {
        "STRAVA_CLIENT_ID": "your_client_id",
        "STRAVA_CLIENT_SECRET": "your_client_secret",
        "STRAVA_REFRESH_TOKEN": "your_refresh_token"
      }
    }
  }
}
```

Or from source:

```json
{
  "mcpServers": {
    "strava": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/src/StravaMcp.Server"],
      "env": {
        "STRAVA_CLIENT_ID": "your_client_id",
        "STRAVA_CLIENT_SECRET": "your_client_secret",
        "STRAVA_REFRESH_TOKEN": "your_refresh_token"
      }
    }
  }
}
```

### With Claude Code

```bash
# Global tool
claude mcp add strava -- strava-mcp-server

# Or from source
claude mcp add strava -- dotnet run --project /path/to/src/StravaMcp.Server
```

Make sure the environment variables are set in your shell before launching.

### Direct / Standalone

```bash
strava-mcp-server
# or: dotnet run --project src/StravaMcp.Server
```

The server communicates via JSON-RPC over stdin/stdout. Diagnostic output goes to stderr.

## Architecture

```
src/StravaMcp.Server/
├── Program.cs              # Entry point, wires up config + tools + server
├── Strava/
│   ├── StravaClient.cs     # HTTP client with auto token refresh
│   ├── StravaConfig.cs     # Configuration model
│   ├── StravaApiException.cs
│   └── TokenData.cs        # Token persistence model
├── Mcp/
│   ├── McpServer.cs        # JSON-RPC stdio server loop
│   ├── JsonRpcMessage.cs   # Request/response models
│   └── ToolDefinition.cs   # Tool + result models
└── Tools/
    ├── IStravaTool.cs      # Tool interface
    ├── ToolHelpers.cs      # Schema builder + argument helpers
    └── *.cs                # One file per tool (21 tools)
```

