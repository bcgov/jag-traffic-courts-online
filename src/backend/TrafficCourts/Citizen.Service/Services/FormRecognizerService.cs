using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

public class FormRecognizerService : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;

    // A mapping list of fields extracted from Azure Form Recognizer and their equivalent JSON name
    private readonly static Dictionary<string, string> FieldLabels = new Dictionary<string, string>()
    {
        { "Violation Ticket Label",     OcrViolationTicket.ViolationTicketTitle },
        { "Violation Ticket Number",    OcrViolationTicket.ViolationTicketNumber },
        { "Surname",                    OcrViolationTicket.Surname },
        { "Given Name",                 OcrViolationTicket.GivenName },
        { "Drivers Licence Number",     OcrViolationTicket.DriverLicenceNumber },
        { "Violation Date",             OcrViolationTicket.ViolationDate },
        { "Violation Time",             OcrViolationTicket.ViolationTime },
        { "Offense is MVA",             OcrViolationTicket.OffenseIsMVA },
        { "Offense is MCA",             OcrViolationTicket.OffenseIsMCA },
        { "Offense is CTA",             OcrViolationTicket.OffenseIsCTA },
        { "Offense is WLA",             OcrViolationTicket.OffenseIsWLA },
        { "Offense is FAA",             OcrViolationTicket.OffenseIsFAA },
        { "Offense is LCA",             OcrViolationTicket.OffenseIsLCA },
        { "Offense is TCR",             OcrViolationTicket.OffenseIsTCR },
        { "Offense is Other",           OcrViolationTicket.OffenseIsOther },
        { "Count 1 Description",        OcrViolationTicket.Count1Description },
        { "Count 1 Act/Regs",           OcrViolationTicket.Count1ActRegs },
        { "Count 1 Is ACT",             OcrViolationTicket.Count1IsACT },
        { "Count 1 Is REGS",            OcrViolationTicket.Count1IsREGS },
        { "Count 1 Section",            OcrViolationTicket.Count1Section },
        { "Count 1 Ticket Amount",      OcrViolationTicket.Count1TicketAmount },
        { "Count 2 Description",        OcrViolationTicket.Count2Description },
        { "Count 2 Act/Regs",           OcrViolationTicket.Count2ActRegs },
        { "Count 2 Is ACT",             OcrViolationTicket.Count2IsACT },
        { "Count 2 Is REGS",            OcrViolationTicket.Count2IsREGS },
        { "Count 2 Section",            OcrViolationTicket.Count2Section },
        { "Count 2 Ticket Amount",      OcrViolationTicket.Count2TicketAmount },
        { "Count 3 Description",        OcrViolationTicket.Count3Description },
        { "Count 3 Act/Regs",           OcrViolationTicket.Count3ActRegs },
        { "Count 3 Is ACT",             OcrViolationTicket.Count3IsACT },
        { "Count 3 Is REGS",            OcrViolationTicket.Count3IsREGS },
        { "Count 3 Section",            OcrViolationTicket.Count3Section },
        { "Count 3 Ticket Amount",      OcrViolationTicket.Count3TicketAmount },
        { "Hearing Location",           OcrViolationTicket.HearingLocation },
        { "Detachment Location",        OcrViolationTicket.DetachmentLocation }
    };

    public FormRecognizerService(String apiKey, Uri endpoint, ILogger<FormRecognizerService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    public async Task<AnalyzeResult> AnalyzeImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        AzureKeyCredential credential = new AzureKeyCredential(_apiKey);
        DocumentAnalysisClient documentAnalysisClient = new DocumentAnalysisClient(_endpoint, credential);

        using Stream stream = GetImageStream(image);
        AnalyzeDocumentOperation analyseDocumentOperation = await documentAnalysisClient.StartAnalyzeDocumentAsync("ViolationTicket", stream, null, cancellationToken);
        await analyseDocumentOperation.WaitForCompletionAsync();

        return analyseDocumentOperation.Value;
    }

    public OcrViolationTicket Map(AnalyzeResult result)
    {
        // Iterate over results, creating a structured document
        OcrViolationTicket violationTicket = new OcrViolationTicket();
        foreach (AnalyzedDocument document in result.Documents)
        {
            violationTicket.GlobalConfidence = document.Confidence;

            if (document.Fields is not null)
            {
                foreach (KeyValuePair<String, DocumentField> pair in document.Fields)
                {
                    // Only map fields of interest
                    if (pair.Key is not null && FieldLabels.Keys.Contains<string>(pair.Key))
                    {
                        OcrViolationTicket.Field field = new OcrViolationTicket.Field();
                        field.Name = pair.Key;
                        field.Value = pair.Value.Content;
                        field.FieldConfidence = pair.Value.Confidence;
                        field.Type = Enum.GetName(pair.Value.ValueType);
                        foreach (BoundingRegion region in pair.Value.BoundingRegions)
                        {
                            OcrViolationTicket.BoundingBox boundingBox = new OcrViolationTicket.BoundingBox();
                            boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[0].X, region.BoundingBox[0].Y));
                            boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[1].X, region.BoundingBox[1].Y));
                            boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[2].X, region.BoundingBox[2].Y));
                            boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[3].X, region.BoundingBox[3].Y));
                            field.BoundingBoxes.Add(boundingBox);
                        }

                        violationTicket.Fields.Add(FieldLabels[pair.Key], field);
                    }
                }
            }
        }
        return violationTicket;
    }

    /// Returns a Stream for the given image
    private Stream GetImageStream(IFormFile image)
    {
        // Work around for a "System.ObjectDisposedException : Cannot access a closed file." error
        // - extract data from the file attachment to an in-memory byte[]
        // - create a new MemoryStream from the byte[]
        byte[] fileBytes;
        using (var fileStream = image.OpenReadStream())
        using (var ms = new MemoryStream())
        {
            fileStream.CopyTo(ms);
            fileBytes = ms.ToArray();
        }
        MemoryStream stream = new MemoryStream(fileBytes);
        return stream;
    }

}
