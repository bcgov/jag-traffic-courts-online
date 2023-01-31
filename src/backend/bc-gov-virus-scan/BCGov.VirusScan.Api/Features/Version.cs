using BCGov.VirusScan.Api.Models;
using BCGov.VirusScan.Api.Services;
using MediatR;

namespace BCGov.VirusScan.Api.Features;

/// <summary>
/// Provides the version feature.
/// </summary>
public static class Version
{
    public class Request : IRequest<GetVersionResult>
    {
        private Request()
        {
        }

        public static readonly Request Instance = new();
    }

    public class Handler : IRequestHandler<Request, GetVersionResult>
    {
        private readonly IVirusScanService _virusScanService;
        private readonly ILogger<Handler> _logger;

        public Handler(IVirusScanService virusScanService, ILogger<Handler> logger)
        {
            _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetVersionResult> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var version = await _virusScanService.GetVersionAsync(cancellationToken);
                return version;
            }
            catch (VersionException)
            {
                throw;
            }
        }
    }
}
