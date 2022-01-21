using Azure.AI.FormRecognizer.DocumentAnalysis;
using MediatR;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;

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
        public AnalyseResponse(OcrViolationTicket violationTicket) {
            OcrViolationTicket = violationTicket;
        }

        public OcrViolationTicket OcrViolationTicket { get; set; }
    }

    public class Handler : IRequestHandler<AnalyseRequest, AnalyseResponse>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IFormRecognizerService _formRegognizerService;
        private readonly IFormRecognizerValidator _formRecognizerValidator;

        public Handler(IFormRecognizerService formRegognizerService, IFormRecognizerValidator formRecognizerValidator, ILogger<Handler> logger)
        {
            _formRegognizerService = formRegognizerService ?? throw new ArgumentNullException(nameof(logger));
            _formRecognizerValidator = formRecognizerValidator ?? throw new ArgumentNullException(nameof(formRecognizerValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Analysing {FileName}", request.Image.FileName);

            AnalyzeResult result = await _formRegognizerService.AnalyzeImageAsync(request.Image, cancellationToken);

            // Create a custom mapping of DocumentFields to a structured object for validation and serialization.
            //   (for some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null), so we map ourselves)
            OcrViolationTicket violationTicket = _formRegognizerService.Map(result);

            // Validate the violationTicket and adjust confidence values (invalid ticket number, invalid count section text, etc)
            _formRecognizerValidator.ValidateViolationTicket(violationTicket);

            AnalyseResponse response = new AnalyseResponse(violationTicket);
            return response;
        }
    }
}
