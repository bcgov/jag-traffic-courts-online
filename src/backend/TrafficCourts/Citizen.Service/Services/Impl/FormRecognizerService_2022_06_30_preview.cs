using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

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
        DocumentAnalysisClient client = new(_endpoint, credential);

        var analyzeResult = await AnalyzeDocumentAsync(client, stream, cancellationToken);

        var result = Map(analyzeResult);
        return result;
    }

    private async Task<AnalyzeResult> AnalyzeDocumentAsync(DocumentAnalysisClient client, Stream form, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.FormRecognizer.BeginOperation("2022_06_30_preview", "RecognizeForms");

        try
        {
            AnalyzeDocumentOperation analyseDocumentOperation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, form, null, cancellationToken)
                .ConfigureAwait(false);

            await analyseDocumentOperation.WaitForCompletionAsync(cancellationToken)
                .ConfigureAwait(false);

            return analyseDocumentOperation.Value;
        }
        catch (Exception exception)
        {
            Instrumentation.FormRecognizer.EndOperation(operation, exception);
            _logger.LogError(exception, "Form Recognizer operation failed");
            throw;
        }
    }

    /// <summary>
    /// Create a custom mapping of DocumentFields to a structured object for validation and serialization.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>
    /// For some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null), so we map ourselves
    /// </remarks>
    private static OcrViolationTicket Map(AnalyzeResult result)
    {
        using Activity? activity = Diagnostics.Source.StartActivity("Map Analyze Result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();
        violationTicket.GlobalConfidence = 0f;
        if (result.Documents is not null && result.Documents.Count > 0)
        {
            violationTicket.GlobalConfidence = result.Documents[0]?.Confidence ?? 0f;
        }

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
                field.Type = Enum.GetName(extractedField.FieldType);
                foreach (BoundingRegion region in extractedField.BoundingRegions)
                {
                    Common.OpenAPIs.OracleDataApi.v1_0.BoundingBox boundingBox = new();
                    boundingBox.Points.Add(new Point(region.BoundingPolygon[0].X, region.BoundingPolygon[0].Y));
                    boundingBox.Points.Add(new Point(region.BoundingPolygon[1].X, region.BoundingPolygon[1].Y));
                    boundingBox.Points.Add(new Point(region.BoundingPolygon[2].X, region.BoundingPolygon[2].Y));
                    boundingBox.Points.Add(new Point(region.BoundingPolygon[3].X, region.BoundingPolygon[3].Y));
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
