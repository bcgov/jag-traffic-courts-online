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
    private readonly static Dictionary<string, string> FieldLabels = new()
    {
        { "Violation Ticket Label",     OcrViolationTicket.ViolationTicketTitle },
        { "Violation Ticket Number",    OcrViolationTicket.ViolationTicketNumber },
        { "Surname",                    OcrViolationTicket.Surname },
        { "Given Name",                 OcrViolationTicket.GivenName },
        { "Drivers Licence Province",   OcrViolationTicket.DriverLicenceProvince },
        { "Drivers Licence Number",     OcrViolationTicket.DriverLicenceNumber },
        { "Violation Date",             OcrViolationTicket.ViolationDate },
        { "Violation Time",             OcrViolationTicket.ViolationTime },
        { "Offence is MVA",             OcrViolationTicket.OffenceIsMVA },
        { "Offence is MCA",             OcrViolationTicket.OffenceIsMCA },
        { "Offence is CTA",             OcrViolationTicket.OffenceIsCTA },
        { "Offence is WLA",             OcrViolationTicket.OffenceIsWLA },
        { "Offence is FAA",             OcrViolationTicket.OffenceIsFAA },
        { "Offence is LCA",             OcrViolationTicket.OffenceIsLCA },
        { "Offence is TCR",             OcrViolationTicket.OffenceIsTCR },
        { "Offence is Other",           OcrViolationTicket.OffenceIsOther },
        { "Count 1 Description",        OcrViolationTicket.Count1Description },
        { "Count 1 Act/Regs",           OcrViolationTicket.Count1ActRegs },
        { "Count 1 is ACT",             OcrViolationTicket.Count1IsACT },
        { "Count 1 is REGS",            OcrViolationTicket.Count1IsREGS },
        { "Count 1 Section",            OcrViolationTicket.Count1Section },
        { "Count 1 Ticket Amount",      OcrViolationTicket.Count1TicketAmount },
        { "Count 2 Description",        OcrViolationTicket.Count2Description },
        { "Count 2 Act/Regs",           OcrViolationTicket.Count2ActRegs },
        { "Count 2 is ACT",             OcrViolationTicket.Count2IsACT },
        { "Count 2 is REGS",            OcrViolationTicket.Count2IsREGS },
        { "Count 2 Section",            OcrViolationTicket.Count2Section },
        { "Count 2 Ticket Amount",      OcrViolationTicket.Count2TicketAmount },
        { "Count 3 Description",        OcrViolationTicket.Count3Description },
        { "Count 3 Act/Regs",           OcrViolationTicket.Count3ActRegs },
        { "Count 3 is ACT",             OcrViolationTicket.Count3IsACT },
        { "Count 3 is REGS",            OcrViolationTicket.Count3IsREGS },
        { "Count 3 Section",            OcrViolationTicket.Count3Section },
        { "Count 3 Ticket Amount",      OcrViolationTicket.Count3TicketAmount },
        { "Hearing Location",           OcrViolationTicket.HearingLocation },
        { "Detachment Location",        OcrViolationTicket.DetachmentLocation }
    };

    public FormRecognizerService(string apiKey, Uri endpoint, ILogger<FormRecognizerService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    public async Task<AnalyzeResult> AnalyzeImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        AzureKeyCredential credential = new(_apiKey);
        DocumentAnalysisClient documentAnalysisClient = new(_endpoint, credential);

        using Stream stream = GetImageStream(image);
        AnalyzeDocumentOperation analyseDocumentOperation = await documentAnalysisClient.StartAnalyzeDocumentAsync("ViolationTicket_v2", stream, null, cancellationToken);
        await analyseDocumentOperation.WaitForCompletionAsync(cancellationToken);

        return analyseDocumentOperation.Value;
    }

    public OcrViolationTicket Map(AnalyzeResult result)
    {
        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();
        violationTicket.GlobalConfidence = result.Documents[0]?.Confidence ?? 0f;

        foreach (var fieldLabel in FieldLabels)
        {
            OcrViolationTicket.Field field = new();
            field.TagName = fieldLabel.Key;
            field.JsonName = fieldLabel.Value;

            DocumentField? extractedField = GetDocumentField(result, fieldLabel.Key);
            if (extractedField is not null)
            {
                field.Value = extractedField.Content;
                field.FieldConfidence = extractedField.Confidence;
                field.Type = Enum.GetName(extractedField.ValueType);
                foreach (BoundingRegion region in extractedField.BoundingRegions)
                {
                    OcrViolationTicket.BoundingBox boundingBox = new();
                    boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[0].X, region.BoundingBox[0].Y));
                    boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[1].X, region.BoundingBox[1].Y));
                    boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[2].X, region.BoundingBox[2].Y));
                    boundingBox.Points.Add(new OcrViolationTicket.Point(region.BoundingBox[3].X, region.BoundingBox[3].Y));
                    field.BoundingBoxes.Add(boundingBox);
                }
            }

            violationTicket.Fields.Add(fieldLabel.Value, field);
        }

        return violationTicket;
    }

    private static DocumentField? GetDocumentField(AnalyzeResult result, string fieldKey)
    {
        if (result.Documents is not null && result.Documents.Count > 0)
        {
            return result.Documents[0].Fields[fieldKey];
        }
        return null;
    }

    /// Returns a Stream for the given image
    private static Stream GetImageStream(IFormFile image)
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
        MemoryStream stream = new(fileBytes);
        return stream;
    }

}
