using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Version;

namespace BCGov.VirusScan.Api.Endpoints;

public class VersionEndpointSummary : Summary<VersionEndpoint>
{
    public VersionEndpointSummary()
    {
        Description = "Gets the ClamAV server and databas version";
        Description = "Sends a VERSION command to the ClamAV server and returns the ClamAV and virus defintion versions.";
        Response<VersionResponse>(StatusCodes.Status200OK);
        Response<VersionResponse>(StatusCodes.Status500InternalServerError);
    }
}
