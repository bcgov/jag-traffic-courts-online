using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators.Rules;

namespace TrafficCourts.Citizen.Service.Validators;

public class FormRecognizerValidator : IFormRecognizerValidator
{

    private static readonly string TicketTitleRegex = @"^VIOLATION TICKET$";
    private static readonly string ViolationTicketNumberRegex = @"^A[A-Z]\d{8}$"; // 2 uppercase characters followed by 8 digits.

    public void ValidateViolationTicket(OcrViolationTicket violationTicket)
    {
        ApplyGlobalRules(violationTicket);
        // abort validation if this is not a valid Violation Ticket.
        if (violationTicket.GlobalValidationErrors.Count > 0)
        {
            return;
        }

        ApplyFieldRules(violationTicket);
    }

    /// <summary>Applies a set of validation rules to determine if the given violationTicket is valid or not.</summary>
    private static void ApplyGlobalRules(OcrViolationTicket violationTicket)
    {
        // TCVP-933 A ticket is considered valid iff
        // - Ticket title reads 'VIOLATION TICKET' at top
        // - Ticket number must start with 'A', another alphabetic character, and then 8 digits
        // - In "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' is selected.
        // - If the Violation Date is less than 30 days
        List<ValidationRule> rules = new();
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle], TicketTitleRegex, ValidationMessages.TicketTitleInvalid));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber], ViolationTicketNumberRegex, ValidationMessages.TicketNumberInvalid));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsMVA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsMCA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsCTA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsWLA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsFAA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsLCA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsTCR]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenseIsOther]));
        rules.Add(new OnlyMVAIsSelectedRule(violationTicket.Fields[OcrViolationTicket.OffenseIsMCA], violationTicket));
        rules.Add(new ViolationDateLT30Rule(violationTicket.Fields[OcrViolationTicket.ViolationDate]));

        // Run each rule and aggregate the results
        rules.ForEach(_ => _.Run());
        foreach (var field in violationTicket.Fields.Values)
        {
            violationTicket.GlobalValidationErrors.AddRange(field.ValidationErrors);
        }

        // drop global Confidence to 0 if this does not appear to be a valid ticket
        if (violationTicket.GlobalValidationErrors.Count > 0)
        {
            violationTicket.GlobalConfidence = 0f;
        }
    }

    private static void ApplyFieldRules(OcrViolationTicket violationTicket)
    {
        List<ValidationRule> rules = new();

        // TCVP-1004 Validate Driver's Licence
        rules.Add(new DriversLicenceValidRule(violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber], violationTicket));
        rules.Add(new TimeRule(violationTicket.Fields[OcrViolationTicket.ViolationTime]));

        rules.ForEach(_ => _.Run());
    }
}
