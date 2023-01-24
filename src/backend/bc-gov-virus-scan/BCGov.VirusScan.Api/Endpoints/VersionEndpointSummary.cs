using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Version;

namespace BCGov.VirusScan.Api.Endpoints;

public class VersionEndpointSummary : Summary<VersionEndpoint>
{
    public VersionEndpointSummary()
    {
        Summary = "Gets the ClamAV server and databas version";
        Description = "Sends a VERSION command to the ClamAV server and returns the ClamAV and virus defintion versions.";
        Response<VersionResponse>(StatusCodes.Status200OK, "The version of ClamAV was retrieved successfully.");
        Response<VersionResponse>(StatusCodes.Status500InternalServerError, "There was an error getting the version from ClamAV.");
    }
}
