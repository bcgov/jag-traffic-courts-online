using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Scan;

namespace BCGov.VirusScan.Api.Endpoints;

public class ScanEndpointSummary : Summary<ScanEndpoint>
{
    public ScanEndpointSummary()
    {
        Summary = "Scans a file for viruses";
        Description = "Sends an INSTREAM command to the ClamAV server and streams the upload file for scanning.";
        Response<ScanResponse>(StatusCodes.Status200OK, "The virus scan operation completed successfully.");
        Response(StatusCodes.Status500InternalServerError, "There was an internal error virus scanning the file.");
    }
}
