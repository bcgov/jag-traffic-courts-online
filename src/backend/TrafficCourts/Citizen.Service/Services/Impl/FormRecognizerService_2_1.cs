using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// This class uses Form Recognizer's FormRecognizerClient to access version 2.1 of the API.
/// </summary>
public class FormRecognizerService_2_1 : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService_2_1> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;
    private readonly string _modelId;
    private readonly double _initialTimeout;
    private readonly double _totalTimeout;

    public FormRecognizerService_2_1(FormRecognizerOptions options, ILogger<FormRecognizerService_2_1> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _apiKey = options.ApiKey ?? throw new ArgumentException($"{nameof(options.ApiKey)} is required");
        _endpoint = options.Endpoint ?? throw new ArgumentException($"{nameof(options.Endpoint)} is required");
        _modelId = options.ModelId ?? throw new ArgumentException($"{nameof(options.ModelId)} is required");
        _initialTimeout = options.InitialTimeout ?? throw new ArgumentException($"{nameof(options.InitialTimeout)} is required");
        _totalTimeout = options.TotalTimeout ?? throw new ArgumentException($"{nameof(options.TotalTimeout)} is required");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OcrViolationTicket> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using Activity? activity = Diagnostics.Source.StartActivity("Analyze Document");
        activity?.AddBaggage("ModelId", _modelId);

        AzureKeyCredential credential = new(_apiKey);
        FormRecognizerClient formRecognizerClient = new(_endpoint, credential);

        Response<RecognizedFormCollection> response = await RecognizeCustomFormsAsync(formRecognizerClient, _modelId, stream, null, cancellationToken);

        return Map(response.Value);
    }


    private async Task<Response<RecognizedFormCollection>> RecognizeCustomFormsAsync(FormRecognizerClient client, string modelId, Stream form, RecognizeCustomFormsOptions? options, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch(); // Used to measure the elapsed time
        stopwatch.Start();

        using var operation = Instrumentation.FormRecognizer.BeginOperation("2.1", "RecognizeForms");
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_totalTimeout)); // Set a total timeout
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        try
        {
            RecognizeCustomFormsOperation operationResult = await client.StartRecognizeCustomFormsAsync(modelId, form, options, linkedCts.Token);

            while (!operationResult.HasCompleted)
            {
                await Task.Delay(1000, linkedCts.Token); // Check status every second
                await UpdateFormsOperationStatus(linkedCts, operationResult);

                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(_initialTimeout) && !HasFormsOperationStarted(operationResult))
                {
                    int elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
                    throw new TimeoutException($"Form Recognizer job failed to start after {elapsedSeconds} seconds.");
                }

                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(_totalTimeout))
                {
                    int elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
                    throw new TimeoutException($"Form Recognizer job timed out after {elapsedSeconds} seconds.");
                }
            }

            return await operationResult.WaitForCompletionAsync(linkedCts.Token).ConfigureAwait(false);
        }
        catch (TimeoutException e)
        {
            Instrumentation.FormRecognizer.EndOperation(operation, e);
            throw;
        }
        catch (OperationCanceledException e)
        {
            Instrumentation.FormRecognizer.EndOperation(operation, e);
            int elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
            throw new TimeoutException($"Form Recognizer job timed out after {elapsedSeconds} seconds.", e);
        }
        catch (Exception e)
        {
            Instrumentation.FormRecognizer.EndOperation(operation, e);
            _logger.LogError(e, "Form Recognizer job failed");
            if (e.Message.StartsWith("Timeout", StringComparison.OrdinalIgnoreCase))
            {
                int elapsedSeconds = (int)Math.Round(stopwatch.Elapsed.TotalSeconds);
                throw new TimeoutException($"Form Recognizer job timed out after {elapsedSeconds} seconds.", e);
            }
            throw;
        }
        finally
        {
            stopwatch.Stop();
            linkedCts.Dispose();
        }
    }

    /// <summary>
    /// Updates the status of a forms operation.
    /// It's been observered that once in a while, Form Recognizer will return a 500 Internal server error while
    /// attempting to retrive the status of the operation. This is due to the fact that the status is stored
    /// in a temporary output.json file on the shared folder in OpenShift. This file is simutanieously used by 
    /// the Layout, API, and Supervised containers and one of them may have a read lock on the file.
    /// 
    /// This method attempts to try 5 times to retrieve the status of the operation if such an error occurs.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="operationResult">The recognize custom forms operation result.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task UpdateFormsOperationStatus(CancellationTokenSource cancellationTokenSource, RecognizeCustomFormsOperation operationResult)
    {
        int retryCount = 0;
        while (retryCount < 5)
        {
            try
            {
                await operationResult.UpdateStatusAsync(cancellationTokenSource.Token);
                return;
            }
            catch (Exception e) when (e.Message.Contains("Internal server error"))
            {
                retryCount++;
                await Task.Delay(1000);
            }
        }
    }

    /// <summary>
    /// Extracts the status from the operationResult's raw response.
    /// </summary>
    /// <param name="operationResult"></param>
    /// <returns></returns>
    private bool HasFormsOperationStarted(RecognizeCustomFormsOperation operationResult)
    {
        Response response = operationResult.GetRawResponse();
        var json = JsonSerializer.Deserialize<FormRecognizerStatus>(response.Content.ToStream());

        return json?.Status != "notStarted";
    }

    private static OcrViolationTicket Map(RecognizedFormCollection result)
    {
        using Activity? activity = Diagnostics.Source.StartActivity("Map Analyze Result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();

        if (result.Count > 0)
        {
            violationTicket.GlobalConfidence = result[0].FormTypeConfidence ?? 0f;
            Dictionary<string, string> fieldLabels = new();
            if (OcrViolationTicket.ViolationTicketVersion1 == result[0].FormType)
            {
                violationTicket.TicketVersion = ViolationTicketVersion.VT1;
                fieldLabels = IFormRecognizerService.FieldLabels_2022_04;
            }
            else if (OcrViolationTicket.ViolationTicketVersion2 == result[0].FormType)
            {
                violationTicket.TicketVersion = ViolationTicketVersion.VT2;
                fieldLabels = IFormRecognizerService.FieldLabels_2023_09;
            }
            foreach (var fieldLabel in fieldLabels)
            {
                Field field = new();
                field.TagName = fieldLabel.Key;
                field.JsonName = fieldLabel.Value;
                var t = field.TagName;

                FormField? extractedField = GetDocumentField(result, fieldLabel.Key);
                if (extractedField is not null)
                {
                    field.FieldConfidence = extractedField.Confidence;
                    field.Type = Enum.GetName(extractedField.Value.ValueType);
                    if (field.Type is not null && field.Type.Equals("SelectionMark"))
                    {
                        // Special case, SelectionMarks. These, it would seem, never populate the ValueData.Text property - always null. We need to extract the value of the field via other means.
                        try
                        {
                            field.Value = Enum.GetName(extractedField.Value.AsSelectionMarkState())?.ToLower();
                        }
                        catch (Exception)
                        {
                            // Could not parse as a SelectionMarkState. When the confidence is so low, there is no valueSelectionMark attribute, so assume "unselected".
                            field.Value = "unselected";
                        }
                    }
                    else
                    {
                        field.Value = extractedField.ValueData?.Text;
                    }

                    FieldData? valueData = extractedField.ValueData;
                    if (valueData is not null)
                    {
                        FieldBoundingBox bb = valueData.BoundingBox;
                        BoundingBox boundingBox = new();
                        boundingBox.Points.Add(new Point(bb[0].X, bb[0].Y));
                        boundingBox.Points.Add(new Point(bb[1].X, bb[1].Y));
                        boundingBox.Points.Add(new Point(bb[2].X, bb[2].Y));
                        boundingBox.Points.Add(new Point(bb[3].X, bb[3].Y));
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
        }

        return violationTicket;
    }

    private static FormField? GetDocumentField(RecognizedFormCollection result, string fieldKey)
    {
        if (result[0] is not null)
        {
            return result[0].Fields[fieldKey];
        }
        return null;
    }

    private class FormRecognizerStatus
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

}
