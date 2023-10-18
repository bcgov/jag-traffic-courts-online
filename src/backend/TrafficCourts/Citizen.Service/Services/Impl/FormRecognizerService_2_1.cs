using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using System.Diagnostics;
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

    public FormRecognizerService_2_1(FormRecognizerOptions options, ILogger<FormRecognizerService_2_1> logger)
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
        FormRecognizerClient formRecognizerClient = new(_endpoint, credential);

        Response<RecognizedFormCollection> response =  await RecognizeCustomFormsAsync(formRecognizerClient, _modelId, stream, null, cancellationToken);

        return Map(response.Value);
    }


    private async Task<Response<RecognizedFormCollection>> RecognizeCustomFormsAsync(FormRecognizerClient client, string modelId, Stream form, RecognizeCustomFormsOptions? options, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.FormRecognizer.BeginOperation("2.1", "RecognizeForms");

        try
        {
            // FIXME: Sometimes Form Recognizer (FR) doesn't even bother to start an analyze request
            //  - the request will be stored in the underlying \shared filesystem, but FR won't start processing the request for some reason.
            //  - the request will sit on the filesystem with the status of NotStarted in an output.json file corresponding with the analyze id.
            //  - it's unclear why this happens, but the result is that the below statement will appear to hang forever and never return, 
            //      but in reality the request is repeatedly polled to see if the request is done and FR always returns NotStarted (there is no current timeout).
            // The WaitForCompletionAsync() should be rewritten so that in such cases if the request hasn't started processing in 10s or so, cancel the request so it doesn't hang here forever. 
            Task<RecognizeCustomFormsOperation> recognizeOperation = client.StartRecognizeCustomFormsAsync(modelId, form, options, cancellationToken);
            Response<RecognizedFormCollection> response = await recognizeOperation.WaitForCompletionAsync(cancellationToken)
                .ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.FormRecognizer.EndOperation(operation, exception);
            _logger.LogError(exception, "Form Recognizer operation failed");
            throw;
        }
    }

    private static OcrViolationTicket Map(RecognizedFormCollection result)
    {
        using Activity? activity = Diagnostics.Source.StartActivity("Map Analyze Result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();

        if (result.Count > 0)
        {
            violationTicket.GlobalConfidence = result[0].FormTypeConfidence ?? 0f;
            Dictionary<string, string> fieldLabels = new ();
            if (OcrViolationTicket.ViolationTicketVersion1 == result[0].FormType) {
                violationTicket.TicketVersion = ViolationTicketVersion.VT1;
                fieldLabels = IFormRecognizerService.FieldLabels_2022_04;
            }
            else if (OcrViolationTicket.ViolationTicketVersion2 == result[0].FormType) {
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
                        field.Value = Enum.GetName(extractedField.Value.AsSelectionMarkState())?.ToLower();
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
}
