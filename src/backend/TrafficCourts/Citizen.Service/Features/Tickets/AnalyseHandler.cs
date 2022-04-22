using Azure.AI.FormRecognizer.DocumentAnalysis;
using MediatR;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common.Features.FilePersistence;

namespace TrafficCourts.Citizen.Service.Features.Tickets;

public static class AnalyseHandler
{
    public class AnalyseRequest : IRequest<AnalyseResponse>
    {
        public AnalyseRequest(IFormFile image)
        {
            ArgumentNullException.ThrowIfNull(image);
            Image = image;
        }

        public IFormFile Image { get; set; }
    }

    public class AnalyseResponse
    {
        public AnalyseResponse(OcrViolationTicket violationTicket)
        {
            ArgumentNullException.ThrowIfNull(violationTicket);
            OcrViolationTicket = violationTicket;
        }

        public OcrViolationTicket OcrViolationTicket { get; set; }
    }

    public class Handler : IRequestHandler<AnalyseRequest, AnalyseResponse>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IFormRecognizerService _formRegognizerService;
        private readonly IFormRecognizerValidator _formRecognizerValidator;
        private readonly IFilePersistenceService _filePersistenceService;
        private readonly IMemoryStreamManager _memoryStreamManager;

        public Handler(
            IFormRecognizerService formRegognizerService,
            IFormRecognizerValidator formRecognizerValidator,
            IFilePersistenceService filePersistenceService,
            IMemoryStreamManager memoryStreamManager,
            ILogger<Handler> logger)
        {
            _formRegognizerService = formRegognizerService ?? throw new ArgumentNullException(nameof(logger));
            _formRecognizerValidator = formRecognizerValidator ?? throw new ArgumentNullException(nameof(formRecognizerValidator));
            _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
            _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Analysing {FileName}", request.Image.FileName);

            var stream = GetStreamForFile(request.Image);

            // FIXME: save should only happen iff there are no global validation errors - this needs to move *after* line 74
            var filename = await _filePersistenceService.SaveFileAsync(stream, cancellationToken);
            stream.Position = 0L; // reset file position

            AnalyzeResult result = await _formRegognizerService.AnalyzeImageAsync(stream, cancellationToken);

            // Create a custom mapping of DocumentFields to a structured object for validation and serialization.
            //   (for some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null), so we map ourselves)
            OcrViolationTicket violationTicket = _formRegognizerService.Map(result);
            violationTicket.ImageFilename = filename;

            // Validate the violationTicket and adjust confidence values (invalid ticket number, invalid count section text, etc)
            _formRecognizerValidator.ValidateViolationTicket(violationTicket);

            AnalyseResponse response = new(violationTicket);
            return response;
        }

        private MemoryStream GetStreamForFile(IFormFile formFile)
        {
            MemoryStream memoryStream = _memoryStreamManager.GetStream(); ;

            using var fileStream = formFile.OpenReadStream();
            fileStream.CopyTo(memoryStream);

            return memoryStream;
        }
    }
}
