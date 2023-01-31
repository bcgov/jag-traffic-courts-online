using BCGov.VirusScan.Api.Models;
using BCGov.VirusScan.Api.Services;
using MediatR;

namespace BCGov.VirusScan.Api.Features;

/// <summary>
/// Provides the scan feature.
/// </summary>
public static class Scan
{
    public class Request : IRequest<Response>
    {
        public IFormFile File { get; private set; }

        public Request(IFormFile file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }
    }

    public class Response
    {
        /// <summary>
        /// Represents an error response.
        /// </summary>
        public static readonly Response Error = new(VirusScanStatus.Error);

        public VirusScanResult Result { get; private set; }

        private Response(VirusScanStatus status)
        {
            Result = new VirusScanResult { Status = status };
        }

        public Response(VirusScanResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            Result = result;
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IVirusScanService _virusScanService;
        private readonly ILogger<Handler> _logger;

        public Handler(IVirusScanService virusScanService, ILogger<Handler> logger)
        {
            _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                using Stream stream = request.File.OpenReadStream();

                VirusScanResult result = await _virusScanService.ScanFileAsync(stream, cancellationToken);

                Response response = new(result);
                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Virus scan file failed with an error");
                return Response.Error;
            }
        }
    }
}
