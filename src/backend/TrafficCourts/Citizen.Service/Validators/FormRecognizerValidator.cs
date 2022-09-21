using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.Features.Lookups;

namespace TrafficCourts.Citizen.Service.Validators;

public class FormRecognizerValidator : IFormRecognizerValidator
{

    private static readonly string _ticketTitleRegex = @"^VIOLATION TICKET$";
    private static readonly string _violationTicketNumberRegex = @"^A[A-Z]\d{8}$"; // 2 uppercase characters followed by 8 digits.
    private readonly IStatuteLookupService _lookupService;

    public FormRecognizerValidator(IStatuteLookupService lookupService)
    {
        ArgumentNullException.ThrowIfNull(lookupService);
        _lookupService = lookupService;
    }

    public async Task ValidateViolationTicketAsync(OcrViolationTicket violationTicket)
    {
        ApplyGlobalRules(violationTicket);
        // abort validation if this is not a valid Violation Ticket.
        if (violationTicket.GlobalValidationErrors.Count > 0)
        {
            return;
        }

        await ApplyFieldRules(violationTicket);

        // TCVP-932 Reject ticket if certain fields have a low confidence value (this is determined after all the fields have been validated and their datatype confirmed)
        LowConfidenceGlobalRule.Run(violationTicket);
    }

    /// <summary>Applies a set of validation rules to determine if the given violationTicket is valid or not.</summary>
    private static async void ApplyGlobalRules(OcrViolationTicket violationTicket)
    {
        // TCVP-933 A ticket is considered valid iff
        // - Ticket title reads 'VIOLATION TICKET' at top
        // - Ticket number must start with 'A', another alphabetic character, and then 8 digits
        // - In "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' is selected.
        // - If the Date of Service is less than 30 days
        // - Count ACT/REGs must be MVA as text - all 3 counts must be MVA at this time.
        List<ValidationRule> rules = new();
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle], _ticketTitleRegex, ValidationMessages.TicketTitleInvalid));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber], _violationTicketNumberRegex, ValidationMessages.TicketNumberInvalid));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsMVA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsMCA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsCTA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsWLA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsFAA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsLCA]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsTCR]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.OffenceIsOther]));
        rules.Add(new OnlyMVAIsSelectedRule(violationTicket.Fields[OcrViolationTicket.OffenceIsMCA], violationTicket));
        rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count1ActRegs], 1));
        rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count2ActRegs], 2));
        rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count3ActRegs], 3));
        rules.Add(new DateOfServiceLT30Rule(violationTicket.Fields[OcrViolationTicket.DateOfService]));

        // Run each rule and aggregate the results
        foreach (var rule in rules)
        {
            await rule.RunAsync();
        }

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

    private async Task ApplyFieldRules(OcrViolationTicket violationTicket)
    {
        List<ValidationRule> rules = new();

        // TCVP-932 Surname and Given Name are required fields
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.GivenName]));
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Surname]));

        // TCVP-1004 Validate Driver's Licence
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince]));
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber]));
        rules.Add(new DriversLicenceValidRule(violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber], violationTicket));

        // Violation Time - ensure the valid can be parsed as a TimeSpan object
        rules.Add(new TimeRule(violationTicket.Fields[OcrViolationTicket.ViolationTime]));

        // Count 1 
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Count1Description]));
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Count1ActRegs]));
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Count1Section]));
        rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Count1TicketAmount]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count1IsACT]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count1IsREGS]));
        rules.Add(new TicketAmountValidRule(violationTicket.Fields[OcrViolationTicket.Count1TicketAmount]));
        rules.Add(new CountSectionRule(violationTicket.Fields[OcrViolationTicket.Count1Section], _lookupService));

        // Count 2 
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count2IsACT]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count2IsREGS]));
        rules.Add(new TicketAmountValidRule(violationTicket.Fields[OcrViolationTicket.Count2TicketAmount]));
        rules.Add(new CountSectionRule(violationTicket.Fields[OcrViolationTicket.Count2Section], _lookupService));

        // Count 3 
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count3IsACT]));
        rules.Add(new CheckboxIsValidRule(violationTicket.Fields[OcrViolationTicket.Count3IsREGS]));
        rules.Add(new TicketAmountValidRule(violationTicket.Fields[OcrViolationTicket.Count3TicketAmount]));
        rules.Add(new CountSectionRule(violationTicket.Fields[OcrViolationTicket.Count3Section], _lookupService));

        foreach (var rule in rules)
        {
            await rule.RunAsync();
        }
    }
}
