using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

public class FormRecognizerService : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;

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
            violationTicket.Confidence = document.Confidence;

            if (document.Fields is not null)
            {
                foreach (KeyValuePair<String, DocumentField> pair in document.Fields)
                {
                    // Only map fields of interest
                    if (pair.Key is not null && OcrViolationTicket.FIELDS.Contains<string>(pair.Key))
                    {
                        OcrViolationTicket.Field field = new OcrViolationTicket.Field();
                        field.Name = pair.Key;
                        field.Value = pair.Value.Content;
                        field.Confidence = pair.Value.Confidence;
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

                        violationTicket.Fields.Add(field);
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
