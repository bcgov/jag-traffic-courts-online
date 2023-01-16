using FastEndpoints;

namespace BCGov.VirusScan.Api.Endpoints;

public class PingEndpointSummary : Summary<PingEndpoint>
{
    public PingEndpointSummary()
    {
        Description = "Pings the ClamAV server";
        Response(StatusCodes.Status200OK, "Ping was successful");
        Response(StatusCodes.Status500InternalServerError);
    }
}
