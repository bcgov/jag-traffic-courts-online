using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// This class uses Form Recognizer's DocumentAnalysisClient to access version 2022-06-30-preview of the API.
/// </summary>
public class FormRecognizerService_2022_06_30_preview : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService_2022_06_30_preview> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;
    private readonly string _modelId;

    public FormRecognizerService_2022_06_30_preview(FormRecognizerOptions options, ILogger<FormRecognizerService_2022_06_30_preview> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _apiKey = options.ApiKey ?? throw new ArgumentException($"{nameof(options.ApiKey)} is required");
        _endpoint = options.Endpoint ?? throw new ArgumentException($"{nameof(options.Endpoint)} is required");
        _modelId = options.ModelId ?? throw new ArgumentException($"{nameof(options.ModelId)} is required");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OcrViolationTicket> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using Activity? activity = Diagnostics.Source.StartActivity("Analyze Document");
        activity?.AddBaggage("ModelId", _modelId);

        AzureKeyCredential credential = new(_apiKey);
        DocumentAnalysisClient documentAnalysisClient = new(_endpoint, credential);
        AnalyzeDocumentOperation analyseDocumentOperation = await documentAnalysisClient.StartAnalyzeDocumentAsync(_modelId, stream, null, cancellationToken);
        await analyseDocumentOperation.WaitForCompletionAsync(cancellationToken);

        return Map(analyseDocumentOperation.Value);
    }

    // Create a custom mapping of DocumentFields to a structured object for validation and serialization.
    //   (for some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null), so we map ourselves)
    private static OcrViolationTicket Map(AnalyzeResult result)
    {
        using Activity? activity = Diagnostics.Source.StartActivity("Map Analyze Result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();
        violationTicket.GlobalConfidence = result.Documents[0]?.Confidence ?? 0f;

        foreach (var fieldLabel in IFormRecognizerService.FieldLabels)
        {
            Field field = new();
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
                    Models.Tickets.BoundingBox boundingBox = new();
                    boundingBox.Points.Add(new Point(region.BoundingBox[0].X, region.BoundingBox[0].Y));
                    boundingBox.Points.Add(new Point(region.BoundingBox[1].X, region.BoundingBox[1].Y));
                    boundingBox.Points.Add(new Point(region.BoundingBox[2].X, region.BoundingBox[2].Y));
                    boundingBox.Points.Add(new Point(region.BoundingBox[3].X, region.BoundingBox[3].Y));
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
}
