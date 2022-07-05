using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.FormRecognizer.Models;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// This class uses Form Recognizer's FormRecognizerClient to access an API version 2.1
/// </summary>
public class FormRecognizerService_2_1 : IFormRecognizerService
{
    private readonly ILogger<FormRecognizerService_2_1> _logger;
    private readonly string _apiKey;
    private readonly Uri _endpoint;
    private readonly string _modelId;

    // A mapping list of fields extracted from Azure Form Recognizer and their equivalent JSON name
    private readonly static Dictionary<string, string> _fieldLabels = new()
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
        { "Detachment Location",        OcrViolationTicket.DetachmentLocation },
        { "Date of Service",            OcrViolationTicket.DateOfService }
    };

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
        Task<RecognizeCustomFormsOperation> operation = formRecognizerClient.StartRecognizeCustomFormsAsync(_modelId, stream, null, cancellationToken);
        Response<RecognizedFormCollection> response = await operation.WaitForCompletionAsync(cancellationToken);

        return Map(response.Value);
    }

    private OcrViolationTicket Map(RecognizedFormCollection result)
    {
        using Activity? activity = Diagnostics.Source.StartActivity("Map Analyze Result");

        // Initialize OcrViolationTicket with all known fields extracted from the Azure Form Recognizer
        OcrViolationTicket violationTicket = new();

        if (result.Count > 0)
        {
            violationTicket.GlobalConfidence = result[0].FormTypeConfidence ?? 0f;

            foreach (var fieldLabel in _fieldLabels)
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
                        Models.Tickets.BoundingBox boundingBox = new();
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
