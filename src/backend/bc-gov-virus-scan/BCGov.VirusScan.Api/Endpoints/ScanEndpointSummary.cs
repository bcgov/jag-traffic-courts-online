using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Scan;

namespace BCGov.VirusScan.Api.Endpoints;

public class ScanEndpointSummary : Summary<ScanEndpoint>
{
    public ScanEndpointSummary()
    {
        Description = "Scans a file for viruses";
        Response<ScanResponse>(200);        
    }
}
