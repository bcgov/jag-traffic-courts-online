using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Diagnostics;
using TrafficCourts.Domain.Models;

using ViolationTicketVersion = TrafficCourts.Domain.Models.ViolationTicketVersion;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// This class uses Form Recognizer's DocumentAnalysisClient to access version 2022-08-31 of the API.
/// </summary>
public class FormRecognizerService_2022_08_31 : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService_2022_08_31> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;
    private readonly string _modelId;
    private readonly double _timeout;

    public FormRecognizerService_2022_08_31(FormRecognizerOptions options, ILogger<FormRecognizerService_2022_08_31> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _apiKey = options.ApiKey ?? throw new ArgumentException($"{nameof(options.ApiKey)} is required");
        _endpoint = options.Endpoint ?? throw new ArgumentException($"{nameof(options.Endpoint)} is required");
        _modelId = options.ModelId ?? throw new ArgumentException($"{nameof(options.ModelId)} is required");
        _timeout = options.Timeout ?? throw new ArgumentException($"{nameof(options.Timeout)} is required");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OcrViolationTicket> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using Activity? activity = Diagnostics.Source.StartActivity("analyze document");
        activity?.AddBaggage("ModelId", _modelId);
        DocumentAnalysisClientOptions clientOptions = new(DocumentAnalysisClientOptions.ServiceVersion.V2022_08_31);

        AzureKeyCredential credential = new(_apiKey);
        DocumentAnalysisClient client = new(_endpoint, credential, clientOptions);

        var analyzeResult = await AnalyzeDocumentAsync(client, stream, cancellationToken);

        var result = Map(analyzeResult);
        return result;
    }

    private async Task<AnalyzeResult> AnalyzeDocumentAsync(DocumentAnalysisClient client, Stream form, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew(); // Used to measure the elapsed time

        using var operation = Instrumentation.FormRecognizer.BeginOperation("2022_08_31", "RecognizeForms");
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeout));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        _logger.LogDebug("Starting form recognition using url: {_endpoint}, modelId: {_modelId}, timeout: {_timeout}", _endpoint, _modelId, _timeout);

        try
        {
            var analyzeOperation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, form, null, linkedCts.Token).ConfigureAwait(false);
            var analyzeResult = await analyzeOperation.WaitForCompletionAsync(linkedCts.Token).ConfigureAwait(false);
            return analyzeResult.Value;
        }
        catch (TimeoutException e)
        {
            throw HandleTimeoutException(operation, stopwatch, e);
        }
        catch (OperationCanceledException e)
        {
            throw HandleTimeoutException(operation, stopwatch, e);
        }
        catch (Exception e) when (IsInternalServerError(e))
        {
            throw HandleInternalServerError(operation, stopwatch, e);
        }
        catch (Exception e)
        {
            throw HandleOtherException(operation, e);
        }
    }

    private bool IsInternalServerError(Exception e)
    {
        return e.Message.StartsWith("Timeout", StringComparison.OrdinalIgnoreCase) && e.Message.Contains("Internal server error");
    }

    private Exception HandleTimeoutException(ITimerOperation operation, Stopwatch stopwatch, Exception e)
    {
        Instrumentation.FormRecognizer.EndOperation(operation, e);
        var elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
        var message = string.Format("Form Recognizer job timed out after {elapsedSeconds} seconds.", elapsedSeconds);
        _logger.LogError(message);
        return new TimeoutException(message, e);
    }

    private Exception HandleInternalServerError(ITimerOperation operation, Stopwatch stopwatch, Exception e)
    {
        // Sometimes the text "Internal server error" is buried in the exception message, but starts with a misleading "Timeout ...".
        // The root cause is not a timeout, but a Form Recognizer server error. So we wrap the exception to make it more clear of the actual cause.
        Instrumentation.FormRecognizer.EndOperation(operation, e);
        var elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
        var message = string.Format("Form Recognizer internal server error after {elapsedSeconds} seconds, likely concurrent/locked file access of shared resources. Job was cancelled.", elapsedSeconds);
        _logger.LogError(message);
        return new Exception(message, e);
    }

    private Exception HandleOtherException(ITimerOperation operation, Exception e)
    {
        Instrumentation.FormRecognizer.EndOperation(operation, e);
        _logger.LogError(e, "Form Recognizer operation failed");
        return e;
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
        using Activity? activity = Diagnostics.Source.StartActivity("map analyze result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();
        violationTicket.GlobalConfidence = 0f;

        Dictionary<string, string> fieldLabels = [];
        if (result.Documents is not null && result.Documents.Count > 0)
        {
            AnalyzedDocument document = result.Documents[0];
            violationTicket.GlobalConfidence = document.Confidence;
            if (document.DocumentType is not null 
                && (OcrViolationTicket.ViolationTicketVersion1_beta == document.DocumentType
                 || document.DocumentType.StartsWith(OcrViolationTicket.ViolationTicketVersion1_x)))
            {
                violationTicket.TicketVersion = ViolationTicketVersion.VT1;
                fieldLabels = IFormRecognizerService.FieldLabels_VT1;
            }
            else if (document.DocumentType is not null 
                && (OcrViolationTicket.ViolationTicketVersion2_beta == document.DocumentType
                 || document.DocumentType.StartsWith(OcrViolationTicket.ViolationTicketVersion2_x)))
            {
                violationTicket.TicketVersion = ViolationTicketVersion.VT2;
                fieldLabels = IFormRecognizerService.FieldLabels_VT2;
            }
            else
            {
                throw new Exception($"Unexpected document type: {document.DocumentType}");
            }
        }

        foreach (var fieldLabel in fieldLabels)
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
                    Domain.Models.BoundingBox boundingBox = new();
                    boundingBox.Points.Add(new Domain.Models.Point(region.BoundingPolygon[0].X, region.BoundingPolygon[0].Y));
                    boundingBox.Points.Add(new Domain.Models.Point(region.BoundingPolygon[1].X, region.BoundingPolygon[1].Y));
                    boundingBox.Points.Add(new Domain.Models.Point(region.BoundingPolygon[2].X, region.BoundingPolygon[2].Y));
                    boundingBox.Points.Add(new Domain.Models.Point(region.BoundingPolygon[3].X, region.BoundingPolygon[3].Y));
                    field.BoundingBoxes.Add(boundingBox);
                }
            }

            violationTicket.Fields.Add(fieldLabel.Value, field);
        }

        // For VT2, ViolationDate is multiple fields and need to be merged together.
        if (ViolationTicketVersion.VT2.Equals(violationTicket.TicketVersion))
        {
            Field violationDateYYYY = violationTicket.Fields[OcrViolationTicket.ViolationDateYYYY];
            Field violationDateMM = violationTicket.Fields[OcrViolationTicket.ViolationDateMM];
            Field violationDateDD = violationTicket.Fields[OcrViolationTicket.ViolationDateDD];

            Field violationDate = new();
            violationDate.TagName = "Violation Date";
            violationDate.JsonName = OcrViolationTicket.ViolationDate;
            violationDate.FieldConfidence = (violationDateYYYY.FieldConfidence + violationDateMM.FieldConfidence + violationDateDD.FieldConfidence) / 3f;
            violationDate.Type = violationDateYYYY.Type;
            violationDate.Value = violationDateYYYY.Value + "-" + violationDateMM.Value + "-" + violationDateDD.Value;
            violationDate.BoundingBoxes.AddRange(violationDateYYYY.BoundingBoxes);
            violationDate.BoundingBoxes.AddRange(violationDateMM.BoundingBoxes);
            violationDate.BoundingBoxes.AddRange(violationDateDD.BoundingBoxes);
            violationTicket.Fields.Add(OcrViolationTicket.ViolationDate, violationDate);

            violationTicket.Fields.Remove(OcrViolationTicket.ViolationDateYYYY);
            violationTicket.Fields.Remove(OcrViolationTicket.ViolationDateMM);
            violationTicket.Fields.Remove(OcrViolationTicket.ViolationDateDD);
        }

        // For VT2, ViolationTime is multiple fields and need to be merged together.
        if (ViolationTicketVersion.VT2.Equals(violationTicket.TicketVersion))
        {
            Field violationTimeHH = violationTicket.Fields[OcrViolationTicket.ViolationTimeHH];
            Field violationTimeMM = violationTicket.Fields[OcrViolationTicket.ViolationTimeMM];

            Field violationTime = new();
            violationTime.TagName = "Violation Time";
            violationTime.JsonName = OcrViolationTicket.ViolationTime;
            violationTime.FieldConfidence = (violationTimeHH.FieldConfidence + violationTimeMM.FieldConfidence) / 2f;
            violationTime.Type = violationTimeHH.Type;
            violationTime.Value = violationTimeHH.Value + ":" + violationTimeMM.Value;
            violationTime.BoundingBoxes.AddRange(violationTimeHH.BoundingBoxes);
            violationTime.BoundingBoxes.AddRange(violationTimeMM.BoundingBoxes);
            violationTicket.Fields.Add(OcrViolationTicket.ViolationTime, violationTime);

            violationTicket.Fields.Remove(OcrViolationTicket.ViolationTimeHH);
            violationTicket.Fields.Remove(OcrViolationTicket.ViolationTimeMM);
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
