using FastEndpoints;
using BCGov.VirusScan.Api.Security;
using BCGov.VirusScan.Api.Services;

namespace BCGov.VirusScan.Api.Endpoints;

public class PingEndpoint : EndpointWithoutRequest
{
    private readonly IVirusScanService _virusScanService;
    private readonly ILogger<PingEndpoint> _logger;

    public PingEndpoint(IVirusScanService virusScanService, ILogger<PingEndpoint> logger)
    {
        _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("ping");
        Options(_ => _.WithTags("ClamAV"));

        if (AuthenticationConfiguration.AllowAnonymous)
        {
            AllowAnonymous();
        }
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {

        bool pong = false;

        try
        {
            pong = await _virusScanService.PingAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing ping");
        }

        if (pong)
        {
            await SendOkAsync();
        }
        else
        {
            await SendErrorsAsync(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
