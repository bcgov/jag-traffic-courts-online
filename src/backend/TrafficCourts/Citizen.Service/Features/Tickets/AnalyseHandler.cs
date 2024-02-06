using MediatR;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

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
        public bool Sanitize { get; set; }
        public bool Validate { get; set; }
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
        private readonly IMemoryStreamManager _memoryStreamManager;
        private readonly IRedisCacheService _redisCacheService;

        public Handler(
            IFormRecognizerService formRegognizerService,
            IFormRecognizerValidator formRecognizerValidator,
            IMemoryStreamManager memoryStreamManager,
            IRedisCacheService redisCacheService,
            ILogger<Handler> logger)
        {
            _formRegognizerService = formRegognizerService ?? throw new ArgumentNullException(nameof(logger));
            _formRecognizerValidator = formRecognizerValidator ?? throw new ArgumentNullException(nameof(formRecognizerValidator));
            _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Analysing {FileName}", request.Image.FileName);

            // Generate a guid with a suffix '-o' to indicate that it's an OCRed ticketId
            // for using as Violation Ticket Key to save OCR related data into Redis
            string ticketId = $"{Guid.NewGuid():n}-o";

            var stream = GetStreamForFile(request.Image);

            // Save the image to redis cache, which will expire after 1 day.
            var filename = await _redisCacheService.SetFileRecordAsync(ticketId, stream, TimeSpan.FromDays(1));
            stream.Position = 0L; // reset file position

            OcrViolationTicket violationTicket;
            try
            {
                violationTicket = await _formRegognizerService.AnalyzeImageAsync(stream, cancellationToken);
                violationTicket.ImageFilename = filename;
            }
            catch (Exception exception)
            {
                // redis will hold the image for 1 day, afterwards, it is removed automatically,
                // so no need to delete from the system.
                await _redisCacheService.DeleteRecordAsync(filename);

                _logger.LogError(exception, "Exception thrown during analysis");
                throw;
            }

            if (request.Sanitize) {
                // Perform basic cleanup from a bad OCR scan.
                await _formRecognizerValidator.SanitizeViolationTicketAsync(violationTicket);
            }

            if (request.Validate) {
                // Validate the violationTicket and adjust confidence values (invalid ticket number, invalid count section text, etc)
                await _formRecognizerValidator.ValidateViolationTicketAsync(violationTicket);
            }

            // Save the violation ticket OCR data into Redis using the generated guid and set it to expire after 1 day from Redis
            await _redisCacheService.SetRecordAsync<OcrViolationTicket>(ticketId, violationTicket, TimeSpan.FromDays(1));

            // Change the image filename to use Violation Ticket Key (guid) in the result
            violationTicket.ImageFilename = ticketId;

            AnalyseResponse response = new(violationTicket);
            return response;
        }

        private MemoryStream GetStreamForFile(IFormFile formFile)
        {
            MemoryStream memoryStream = _memoryStreamManager.GetStream();

            using var fileStream = formFile.OpenReadStream();
            fileStream.CopyTo(memoryStream);

            return memoryStream;
        }
    }
}
