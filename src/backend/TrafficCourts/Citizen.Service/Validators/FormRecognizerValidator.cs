using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using System.Text.RegularExpressions;

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
        Sanitize(violationTicket);

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

    /// <summary>
    /// Cleans up the scanned data from a poor OCR scan.
    /// </summary>
    /// <param name="violationTicket"></param>
    public static void Sanitize(OcrViolationTicket violationTicket)
    {
        // It can happen that if adjacent text fields has content too close to the common dividing line, the OCR tool can misread both fields thinking one is blank and the other starts with the blank field's text.

        // If the Acts/Regs section is blank (should be MVA or MVR) and the adjacent Section text starts with "MVA/R" (shouldn't start with MVA/R), then move the MVA/R text to the correct field.
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count1Section)
            && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count1ActRegs))
        {
            string count1Section = violationTicket.Fields[OcrViolationTicket.Count1Section].Value ?? "";
            if (!violationTicket.Fields[OcrViolationTicket.Count1ActRegs].IsPopulated()
                && count1Section.StartsWith("M") && count1Section.Length >= 3)
            {
                string actReg = count1Section.Substring(1, 3);
                violationTicket.Fields[OcrViolationTicket.Count1ActRegs].Value = actReg;
                violationTicket.Fields[OcrViolationTicket.Count1Section].Value = count1Section.Replace((actReg is not null ? actReg : ""), "")?.Trim();
            }
        }

        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count2Section)
            && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count2ActRegs))
        {
            string count2Section = violationTicket.Fields[OcrViolationTicket.Count2Section].Value ?? "";
            if (!violationTicket.Fields[OcrViolationTicket.Count2ActRegs].IsPopulated()
                && count2Section.StartsWith("M") && count2Section.Length >= 3)
            {
                string actReg = count2Section.Substring(1, 3);
                violationTicket.Fields[OcrViolationTicket.Count2ActRegs].Value = actReg;
                violationTicket.Fields[OcrViolationTicket.Count2Section].Value = count2Section.Replace((actReg is not null ? actReg : ""), "")?.Trim();
            }
        }

        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count3Section)
            && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count3ActRegs))
        {
            string count3Section = violationTicket.Fields[OcrViolationTicket.Count3Section].Value ?? "";
            if (!violationTicket.Fields[OcrViolationTicket.Count3ActRegs].IsPopulated()
                && count3Section.StartsWith("M") && count3Section.Length >= 3)
            {
                string actReg = count3Section.Substring(1, 3);
                violationTicket.Fields[OcrViolationTicket.Count3ActRegs].Value = actReg;
                violationTicket.Fields[OcrViolationTicket.Count1Section].Value = count3Section.Replace((actReg is not null ? actReg : ""), "")?.Trim();
            }
        }

        // If any act regs starts with MU replace with MV since the U is likely a mis-scan of the V
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count1ActRegs))
            violationTicket.Fields[OcrViolationTicket.Count1ActRegs].Value = violationTicket.Fields[OcrViolationTicket.Count1ActRegs].Value?.Replace("MU", "MV");
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count2ActRegs))
            violationTicket.Fields[OcrViolationTicket.Count2ActRegs].Value = violationTicket.Fields[OcrViolationTicket.Count2ActRegs].Value?.Replace("MU", "MV");
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count3ActRegs))
            violationTicket.Fields[OcrViolationTicket.Count3ActRegs].Value = violationTicket.Fields[OcrViolationTicket.Count3ActRegs].Value?.Replace("MU", "MV");

        // Pre-process ticket number if scan reads an A Oh as a A Zero replace the Zero with an O in the second position
        // replace A1 with AI
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationTicketNumber))
        {
            string? ticketNumber = violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value;
            if (ticketNumber is not null && ticketNumber.StartsWith("A0"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A0", "AO");
            }
            if (ticketNumber is not null && ticketNumber.StartsWith("A1"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A1", "AI");
            }
        }

        // Pre-process description and act/regs fields for counts if description number contains text of MVA or MVR strip it out and replace blank act/regs field text
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count1ActRegs) && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count1Description))
        {
            string? desc = violationTicket.Fields[OcrViolationTicket.Count1Description].Value;
            bool actRegPop = violationTicket.Fields[OcrViolationTicket.Count1ActRegs].IsPopulated();
            if (desc is not null && (desc.Contains("MVA") || desc.Contains("MVR")) && actRegPop == false)
            {
                string actReg = "MVA";
                if (desc.Contains("MVR")) actReg = "MVR";
                violationTicket.Fields[OcrViolationTicket.Count1Description].Value = violationTicket.Fields[OcrViolationTicket.Count1Description].Value?.Replace(actReg, "");
                violationTicket.Fields[OcrViolationTicket.Count1ActRegs].Value = actReg;
            }
        }
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count2ActRegs) && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count2Description))
        {
            string? desc = violationTicket.Fields[OcrViolationTicket.Count2Description].Value;
            bool actRegPop = violationTicket.Fields[OcrViolationTicket.Count2ActRegs].IsPopulated();
            if (desc is not null && (desc.Contains("MVA") || desc.Contains("MVR")) && actRegPop == false)
            {
                string actReg = "MVA";
                if (desc.Contains("MVR")) actReg = "MVR";
                violationTicket.Fields[OcrViolationTicket.Count2Description].Value = violationTicket.Fields[OcrViolationTicket.Count2Description].Value?.Replace(actReg, "");
                violationTicket.Fields[OcrViolationTicket.Count2ActRegs].Value = actReg;
            }
        }
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.Count3ActRegs) && violationTicket.Fields.ContainsKey(OcrViolationTicket.Count3Description))
        {
            string? desc = violationTicket.Fields[OcrViolationTicket.Count3Description].Value;
            bool actRegPop = violationTicket.Fields[OcrViolationTicket.Count3ActRegs].IsPopulated();
            if (desc is not null && (desc.Contains("MVA") || desc.Contains("MVR")) && actRegPop == false)
            {
                string actReg = "MVA";
                if (desc.Contains("MVR")) actReg = "MVR";
                violationTicket.Fields[OcrViolationTicket.Count3Description].Value = violationTicket.Fields[OcrViolationTicket.Count3Description].Value?.Replace(actReg, "");
                violationTicket.Fields[OcrViolationTicket.Count3ActRegs].Value = actReg;
            }
        }

        // Sometimes reads province into drivers licence number at the front of the text for DL number
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.DriverLicenceNumber) && violationTicket.Fields.ContainsKey(OcrViolationTicket.DriverLicenceProvince))
        {
            if (!violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].IsPopulated()) // blank DL province
            {
                string dlNumber = violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value ?? "";
                if (Regex.IsMatch(dlNumber, @"^[a-zA-Z][a-zA-Z]")) // DL number starts with two chars

                {
                    violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value = dlNumber.Substring(2, dlNumber.Length - 2).Trim();
                    violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].Value = dlNumber.Substring(0, 2); // extract first two chars for province code
                }
            }
        }

        // Sanitize service date and if its not a date set it to violation date (also sanitized)
        // TCVP-1645 Use DateOfService if available, otherwise fallback to ViolationDate
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationDate))
        {
            violationTicket.Fields[OcrViolationTicket.ViolationDate].Value = violationTicket.Fields[OcrViolationTicket.ViolationDate].GetDate()?.ToString("yyyy-MM-dd");
        }
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.DateOfService))
        {
            if (violationTicket.Fields[OcrViolationTicket.DateOfService].IsPopulated()) // if date of service is present try putting it in good format
                violationTicket.Fields[OcrViolationTicket.DateOfService].Value = violationTicket.Fields[OcrViolationTicket.DateOfService].GetDate()?.ToString("yyyy-MM-dd");

            // if formatting date of service didnt work or its null or not populated set it to violation date
            if (!violationTicket.Fields[OcrViolationTicket.DateOfService].IsPopulated() || violationTicket.Fields[OcrViolationTicket.DateOfService] is null)
                violationTicket.Fields[OcrViolationTicket.DateOfService].Value = violationTicket.Fields[OcrViolationTicket.ViolationDate].GetDate()?.ToString("yyyy-MM-dd");
        }

    }

    /// <summary>Applies a set of validation rules to determine if the given violationTicket is valid or not.</summary>
    private static async void ApplyGlobalRules(OcrViolationTicket violationTicket)
    {
        // TCVP-933 A ticket is considered valid iff
        // - Ticket title reads 'VIOLATION TICKET' at top
        // - Ticket number must start with 'A', another alphabetic character, and then 8 digits
        // - If the Date of Service is less than 30 days
        List<ValidationRule> rules = new();
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle], _ticketTitleRegex, ValidationMessages.TicketTitleInvalid));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber], _violationTicketNumberRegex, ValidationMessages.TicketNumberInvalid));
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
