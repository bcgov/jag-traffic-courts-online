using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators.Rules;

namespace TrafficCourts.Citizen.Service.Validators;

public class FormRecognizerValidator : IFormRecognizerValidator
{

    private static readonly string TicketTitleRegex = @"^VIOLATION TICKET$";
    private static readonly string ViolationTicketNumberRegex = @"^A[A-Z]\d{8}$"; // 2 uppercase characters followed by 8 digits.

    public void ValidateViolationTicket(OcrViolationTicket violationTicket)
    {
        // TCVP-933 A ticket is considered valid iff
        // - Ticket title reads 'VIOLATION TICKET' at top
        // - Ticket number must start with 'A', another alphabetic character, and then 8 digits
        // - In "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' is selected.
        // - If the Violation Date is less than 30 days
        List<ValidationRule> rules = new List<ValidationRule>();
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle], TicketTitleRegex, ValidationMessages.TicketTitleInvalid));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber], ViolationTicketNumberRegex, ValidationMessages.TicketNumberInvalid));
        // TODO: Add OnlyMCAIsSelectedRule
        // TODO: Add ViolationDateLT30Rule

        // Run each rule and aggregate the results
        rules.ForEach(_ =>
        {
            _.Run();
            violationTicket.ValidationErrors.AddRange(_.ValidationErrors);
        });

        // drop global Confidence to 0 if this does not appear to be a valid ticket
        if (violationTicket.ValidationErrors.Count > 0)
        {
            violationTicket.Confidence = 0f;
        }
    }
}
