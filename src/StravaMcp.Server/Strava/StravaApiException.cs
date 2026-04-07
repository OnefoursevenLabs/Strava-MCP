namespace StravaMcp.Server.Strava;

public sealed class StravaApiException : Exception
{
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public StravaApiException(int statusCode, string responseBody)
        : base($"Strava API error {statusCode}: {responseBody}")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
