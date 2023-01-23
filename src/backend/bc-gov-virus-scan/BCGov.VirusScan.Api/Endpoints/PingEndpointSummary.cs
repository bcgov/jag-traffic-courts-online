using FastEndpoints;

namespace BCGov.VirusScan.Api.Endpoints;

public class PingEndpointSummary : Summary<PingEndpoint>
{
    public PingEndpointSummary()
    {
        Summary = "Pings the ClamAV server";     
        Description = "Sends a PING command to the ClamAV server";
        Response(StatusCodes.Status200OK, "Ping was successful");
        Response(StatusCodes.Status500InternalServerError);
    }
}
