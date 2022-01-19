using Azure.AI.FormRecognizer.DocumentAnalysis;
using MediatR;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;

namespace TrafficCourts.Citizen.Service.Features.Tickets;

public static class AnalyseHandler
{
    public class AnalyseRequest : IRequest<AnalyseResponse>
    {
        public AnalyseRequest(IFormFile image)
        {
            this.Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public IFormFile Image { get; set; }
    }

    public class AnalyseResponse
    {
        public OcrViolationTicket? OcrViolationTicket { get; set; }
    }

    public class Handler : IRequestHandler<AnalyseRequest, AnalyseResponse>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IFormRecognizerService _formRegognizerService;

        public Handler(IFormRecognizerService formRegognizerService, ILogger<Handler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _formRegognizerService = formRegognizerService ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Analysing {FileName}", request.Image.FileName);

            AnalyzeResult result = await _formRegognizerService.AnalyzeImageAsync(request.Image, cancellationToken);

            // Create a custom mapping of DocumentFields to a structured object for validation and serialization.
            //   (for some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null), so we map ourselves)
            OcrViolationTicket violationTicket = _formRegognizerService.Map(result);

            // TODO: validate violationTicket and adjust confidence values (invalid ticket number, invalid count section text, etc)

            AnalyseResponse response = new AnalyseResponse();
            response.OcrViolationTicket = violationTicket;
            return response;
        }
    }
}
