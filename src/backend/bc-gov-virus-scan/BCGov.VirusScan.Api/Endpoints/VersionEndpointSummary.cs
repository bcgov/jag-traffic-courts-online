using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Version;

namespace BCGov.VirusScan.Api.Endpoints;

public class VersionEndpointSummary : Summary<VersionEndpoint>
{
    public VersionEndpointSummary()
    {
        Description = "Gets the ClamAV server and databas version";
        Response<VersionResponse>(200);
        Response<VersionResponse>(500);
    }
}
