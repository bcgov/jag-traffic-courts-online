using BCGov.VirusScan.Api.Services;
using MediatR;

namespace BCGov.VirusScan.Api.Features;

/// <summary>
/// Provides the ping feature.
/// </summary>
public static class Ping
{
    public class Request : IRequest<bool>
    {
        private Request()
        {
        }

        /// <summary>
        /// Single instance of the the <see cref="Ping.Request"/>
        /// </summary>
        public static readonly Request Instance = new();
    }

    public class Handler : IRequestHandler<Request, bool>
    {
        private readonly IVirusScanService _virusScanService;
        private readonly ILogger<Handler> _logger;

        public Handler(IVirusScanService virusScanService, ILogger<Handler> logger)
        {
            _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                bool pong = await _virusScanService.PingAsync(cancellationToken);
                return pong;
            }
            catch (PingException)
            {
                // virus scan service will have logged the error already, just return false
                return false;
            }
        }
    }
}
