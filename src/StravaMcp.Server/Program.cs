using StravaMcp.Server.Mcp;
using StravaMcp.Server.Strava;
using StravaMcp.Server.Tools;

// Read config from environment variables
var clientId = Environment.GetEnvironmentVariable("STRAVA_CLIENT_ID")
    ?? throw new InvalidOperationException("STRAVA_CLIENT_ID environment variable is required");
var clientSecret = Environment.GetEnvironmentVariable("STRAVA_CLIENT_SECRET")
    ?? throw new InvalidOperationException("STRAVA_CLIENT_SECRET environment variable is required");
var refreshToken = Environment.GetEnvironmentVariable("STRAVA_REFRESH_TOKEN")
    ?? throw new InvalidOperationException("STRAVA_REFRESH_TOKEN environment variable is required");
var tokenFile = Environment.GetEnvironmentVariable("STRAVA_TOKEN_FILE") ?? "strava_tokens.json";

var config = new StravaConfig
{
    ClientId = clientId,
    ClientSecret = clientSecret,
    RefreshToken = refreshToken,
    TokenFilePath = tokenFile
};

using var client = new StravaClient(config);

var tools = new List<IStravaTool>
{
    // Athlete
    new GetAthleteTool(client),
    new GetAthleteStatsTool(client),

    // Activities
    new ListActivitiesTool(client),
    new GetActivityTool(client),
    new GetActivityLapsTool(client),
    new GetActivityZonesTool(client),
    new GetActivityCommentsTool(client),
    new GetActivityKudosTool(client),
    new GetActivityStreamsTool(client),
    new UpdateActivityTool(client),

    // Segments
    new GetSegmentTool(client),
    new GetSegmentEffortsTool(client),
    new ExploreSegmentsTool(client),
    new StarredSegmentsTool(client),

    // Routes
    new ListRoutesTool(client),
    new GetRouteTool(client),

    // Gear
    new GetGearTool(client),

    // Clubs
    new ListClubsTool(client),
    new GetClubTool(client),
    new GetClubActivitiesTool(client),
    new GetClubMembersTool(client),
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var server = new McpServer(tools);

// Log to stderr so it doesn't interfere with JSON-RPC on stdout
Console.Error.WriteLine("Strava MCP Server started (21 tools available)");

await server.RunAsync(cts.Token);
