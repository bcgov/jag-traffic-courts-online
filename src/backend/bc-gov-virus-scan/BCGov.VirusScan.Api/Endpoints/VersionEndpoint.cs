using FastEndpoints;
using BCGov.VirusScan.Api.Contracts.Version;
using BCGov.VirusScan.Api.Security;
using BCGov.VirusScan.Api.Services;

namespace BCGov.VirusScan.Api.Endpoints;

public class VersionEndpoint : EndpointWithoutRequest<VersionResponse>
{
    private readonly IVirusScanService _virusScanService;

    public VersionEndpoint(IVirusScanService virusScanService)
    {
        _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
    }
    public override void Configure()
    {
        Get("version");

        if (AuthenticationConfiguration.AllowAnonymous)
        {
            AllowAnonymous();
        }
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var version = await _virusScanService.GetVersionAsync(cancellationToken);

        await SendOkAsync(new VersionResponse(version.SoftwareVersion, version.DatabaseVersion, version.DatabaseDate), cancellationToken);
    }
}
