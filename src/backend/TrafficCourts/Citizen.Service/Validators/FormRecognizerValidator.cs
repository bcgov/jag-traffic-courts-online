using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.Features.Lookups;
using System.Text.RegularExpressions;
using TrafficCourts.Domain.Models;

using OcrViolationTicket = TrafficCourts.Domain.Models.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators;

public class FormRecognizerValidator : IFormRecognizerValidator
{

    private static readonly string _ticketTitleRegex = @"^VIOLATION TICKET$";
    private static readonly string _violationTicketNumberRegex = @"^A[A-Z]\d{8}$"; // 2 uppercase characters followed by 8 digits.
    private readonly IStatuteLookupService _lookupService;
    private readonly ILogger<FormRecognizerValidator> _logger;

    public FormRecognizerValidator(IStatuteLookupService lookupService, ILogger<FormRecognizerValidator> logger)
    {
        ArgumentNullException.ThrowIfNull(lookupService);
        _lookupService = lookupService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SanitizeViolationTicketAsync(OcrViolationTicket violationTicket)
    {
        await SanitizeAsync(violationTicket);
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

    /// <summary>
    /// Cleans up the scanned data from a poor OCR scan.
    /// </summary>
    /// <param name="violationTicket"></param>
    public async Task SanitizeAsync(OcrViolationTicket violationTicket)
    {
        // TODO: Use TryGetValue to avoid numerous trips back and forth into the dictionary
        // It can happen that if adjacent text fields has content too close to the common dividing line, the OCR tool can misread both fields thinking one is blank and the other starts with the blank field's text.
        // If the Acts/Regs section is blank (should be MVA or MVR) and the adjacent Section text starts with "MVA/R" (shouldn't start with MVA/R), then move the MVA/R text to the correct field.
        void SplitSectionActRegs(string sectionKey, string actRegsKey)
        {
            if (violationTicket.Fields.ContainsKey(sectionKey) && violationTicket.Fields.ContainsKey(actRegsKey))
            {
                string countSection = violationTicket.Fields[sectionKey].Value ?? "";
                if (!violationTicket.Fields[actRegsKey].IsPopulated()
                    && countSection.StartsWith("M") && countSection.Length >= 3)
                {
                    string actReg = countSection.Substring(0, 3);
                    violationTicket.Fields[actRegsKey].Value = actReg;
                    violationTicket.Fields[sectionKey].Value = countSection.Replace((actReg is not null ? actReg : ""), "")?.Trim();
                }
            }
        }

        // If any act regs starts with MU replace with MV since the U is likely a mis-scan of the V
        void ReplaceMUWithMV(string actRegsKey)
        {
            if (violationTicket.Fields.ContainsKey(actRegsKey))
                violationTicket.Fields[actRegsKey].Value = violationTicket.Fields[actRegsKey].Value?.Replace("MU", "MV");
        }

        // Pre-process description and act/regs fields for counts if description number contains text of MVA or MVR strip it out and replace blank act/regs field text
        void GetActRegsFromDescription(string descKey, string actRegsKey)
        {
            if (violationTicket.Fields.ContainsKey(actRegsKey) && violationTicket.Fields.ContainsKey(descKey))
            {
                string? desc = violationTicket.Fields[descKey].Value;
                bool actRegPop = violationTicket.Fields[actRegsKey].IsPopulated();
                if (desc is not null && (desc.Contains("MVA") || desc.Contains("MVR")) && actRegPop == false)
                {
                    string actReg = "MVA";
                    if (desc.Contains("MVR")) actReg = "MVR";
                    violationTicket.Fields[descKey].Value = violationTicket.Fields[descKey].Value?.Replace(actReg, "");
                    violationTicket.Fields[actRegsKey].Value = actReg;
                }
            }
        }

        // For each count do the the three sanitization
        void SanitizeCount(string sectionKey, string actRegsKey, string descKey, string ticketAmount)
        {
            SplitSectionActRegs(sectionKey, actRegsKey);
            ReplaceMUWithMV(actRegsKey);
            GetActRegsFromDescription(descKey, actRegsKey);

            // Sanitize Section. It can happen that if the section text is too close to the ticket amount, the text can have a trailing $ symbol (which is meant for the amount field)
            if (violationTicket.Fields.ContainsKey(sectionKey))
            {
                string sectionValue = violationTicket.Fields[sectionKey].Value ?? "";

                // remove all whitespace characters
                string newValue = Regex.Replace(sectionValue, @"\s", "");

                int index = newValue.IndexOf("$");
                if (index > 0)
                {
                    newValue = newValue.Substring(0, index);
                }

                violationTicket.Fields[sectionKey].Value = newValue;
            }

            // sanitize ticket amount, remove whitespace and [trailing] hypens
            if (violationTicket.Fields.ContainsKey(ticketAmount))
            {
                string ticketAmountValue = violationTicket.Fields[ticketAmount].Value ?? "";
                violationTicket.Fields[ticketAmount].Value = Regex.Replace(ticketAmountValue, @"\s", "").Replace("-", "");
            }

            // If the description is populated but the section and ticketAmount fields are blank, clear out the description as it was probably misread from the vertical text on the ticket image.
            if (violationTicket.Fields.ContainsKey(descKey) && violationTicket.Fields[descKey].IsPopulated() 
                && violationTicket.Fields.ContainsKey(sectionKey) && !violationTicket.Fields[sectionKey].IsPopulated()
                && violationTicket.Fields.ContainsKey(ticketAmount) && !violationTicket.Fields[ticketAmount].IsPopulated())
            {
                violationTicket.Fields[descKey].Value = null;
            }

        }
        
        void SanitizeWhiteSpace(string sectionKey)
        {
            if (violationTicket.Fields.TryGetValue(sectionKey, out var field))
            {
                var titleValue = field.Value ?? "";
                field.Value = Regex.Replace(titleValue, @"[\s\0\b\a]+", " "); // replace all whitespace characters with a space
            }
        }

        // TCVP-2914 replace newline characters from location
        SanitizeWhiteSpace(OcrViolationTicket.DetachmentLocation);
        SanitizeWhiteSpace(OcrViolationTicket.HearingLocation);
        SanitizeWhiteSpace(OcrViolationTicket.ViolationTicketTitle);

        SanitizeCount(OcrViolationTicket.Count1Section, OcrViolationTicket.Count1ActRegs, OcrViolationTicket.Count1Description, OcrViolationTicket.Count1TicketAmount);
        SanitizeCount(OcrViolationTicket.Count2Section, OcrViolationTicket.Count2ActRegs, OcrViolationTicket.Count2Description, OcrViolationTicket.Count2TicketAmount);
        SanitizeCount(OcrViolationTicket.Count3Section, OcrViolationTicket.Count3ActRegs, OcrViolationTicket.Count3Description, OcrViolationTicket.Count3TicketAmount);

        // Pre-process ticket number if scan reads an A Oh as A Zero replace the Zero with an O in the second position
        // replace A1 with AI
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationTicketNumber))
        {
            string? ticketNumber = violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value;
            if (ticketNumber is not null && ticketNumber.StartsWith("A0"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A0", "AO");
            }
            else if (ticketNumber is not null && ticketNumber.StartsWith("A1"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A1", "AI");
            }
            else if (ticketNumber is not null && ticketNumber.StartsWith("A5"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A5", "AS");
            }
            else if (ticketNumber is not null && ticketNumber.StartsWith("A8"))
            {
                violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber].Value = ticketNumber.Replace("A8", "AB");
            }
        }

        // Sometimes reads province into drivers licence number at the front of the text for DL number
        // if DL province not populated and DL number starts with two chars take first two chars from DL number
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.DriverLicenceNumber) && violationTicket.Fields.ContainsKey(OcrViolationTicket.DriverLicenceProvince))
        {
            string dlNumber = "";

            if (violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].IsPopulated() 
                && !violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].IsPopulated()) // blank DL number
            {
                dlNumber = violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].Value ?? "";
            }
            else if (!violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].IsPopulated() // blank DL province
                && violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].IsPopulated())
            {
                dlNumber = violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value ?? "";
            }
            if (Regex.IsMatch(dlNumber, @"^[a-zA-Z]{2}\s*\d+")) // DL number starts with two chars followed by digits
            {
                violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value = dlNumber.Substring(2, dlNumber.Length - 2).Trim();
                violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].Value = dlNumber.Substring(0, 2); // extract first two chars for province code
            }
        }

        // Sanitize service date and if its not a date set it to violation date (also sanitized)
        // TCVP-1645 Use DateOfService if available, otherwise fallback to ViolationDate
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationDate))
        {
            // Replace letters with likely digits
            string violationDate = violationTicket.Fields[OcrViolationTicket.ViolationDate].Value ?? "";
            violationDate = violationDate.Replace("O", "0");
            violationDate = violationDate.Replace("o", "0");
            violationDate = violationDate.Replace("l", "1");
            violationDate = violationDate.Replace("f", "1");
            violationTicket.Fields[OcrViolationTicket.ViolationDate].Value = violationDate;
            violationTicket.Fields[OcrViolationTicket.ViolationDate].Value = violationTicket.Fields[OcrViolationTicket.ViolationDate].GetDate()?.ToString("yyyy-MM-dd");
        }
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.DateOfService))
        {
            // Replace letters with likely digits
            string dateOfService = violationTicket.Fields[OcrViolationTicket.DateOfService].Value ?? "";
            dateOfService = dateOfService.Replace("O", "0");
            dateOfService = dateOfService.Replace("o", "0");
            dateOfService = dateOfService.Replace("l", "1");
            dateOfService = dateOfService.Replace("f", "1");
            violationTicket.Fields[OcrViolationTicket.DateOfService].Value = dateOfService;
            violationTicket.Fields[OcrViolationTicket.DateOfService].Value = violationTicket.Fields[OcrViolationTicket.DateOfService].GetDate()?.ToString("yyyy-MM-dd");
        }

        // ViolationDate and DateOfService are usually the same day. If either is misread/null, try using the other date value.
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationDate) && violationTicket.Fields.ContainsKey(OcrViolationTicket.DateOfService))
        {
            if (violationTicket.Fields[OcrViolationTicket.ViolationDate].Value is null
                && violationTicket.Fields[OcrViolationTicket.DateOfService].Value is not null)
            {
                violationTicket.Fields[OcrViolationTicket.ViolationDate].Value = violationTicket.Fields[OcrViolationTicket.DateOfService].Value;
            }
            if (violationTicket.Fields[OcrViolationTicket.DateOfService].Value is null
                && violationTicket.Fields[OcrViolationTicket.ViolationDate].Value is not null)
            {
                violationTicket.Fields[OcrViolationTicket.DateOfService].Value = violationTicket.Fields[OcrViolationTicket.ViolationDate].Value;
            }
        }

        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.ViolationTime))
        {
            // remove trailing colon (:) character
            // It can happen that the hour portion actually contains the entire HH:MM time and the time portion is blank
            // In this scenario when the time is built up from the individual components, we end up with HH:MM:

            string violationTime = violationTicket.Fields[OcrViolationTicket.ViolationTime].Value ?? "";
            violationTime = violationTime.TrimEnd(new char[] { ':' });
            violationTime = violationTime.Replace("O", "0");
            violationTime = violationTime.Replace("o", "0");
            violationTime = violationTime.Replace("l", "1");
            violationTime = violationTime.Replace("f", "1");
            violationTicket.Fields[OcrViolationTicket.ViolationTime].Value = violationTime;
        }

        // Sanitize driver's licence number (sometimes contains spaces - should be digits only)
        if (violationTicket.Fields.ContainsKey(OcrViolationTicket.DriverLicenceNumber))
        {
            string? value = violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value;
            if (value is not null)
            {
                // replace all non digits with null
                string newValue = Regex.Replace(value, @"\D", "");
                violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value = newValue;
            }
        }

        // TCVP-2706 - It appears the Form Recognizer does a poor job at recognizing checkboxes.
        // Instead, use the counts to lookup statutes and if they are valid, replace MVA/MVAR Did Commit checkbox selections with identified Statute act codes.
        await SanitizeDidCommitAsyc(violationTicket);
    }

    // A function to sanitize the OcrViolationTicket and set the isMVA and isMVAR checkboxes based on the validity of the MVA and MVAR Statutes referenced in the section text for the 3 counts.
    private async Task SanitizeDidCommitAsyc(OcrViolationTicket violationTicket)
    {
        // TCVO-2804 - only attempt to use statutes if the MVA/MVAR checkboxes could not be read.

        // If the section text for any of the 3 counts references a valid MVA Statute, the isMVA checkbox should be selected
        if (violationTicket.Fields.TryGetValue(OcrViolationTicket.OffenceIsMVA, out Field? isMVA)) {
            // If field neither, it couldn't be read successfully, attempt to use statutes instead.
            if (isMVA.Value != Field._selected && isMVA.Value != Field._unselected) { 
                isMVA.Value = (
                    await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count1Section, Field._mva)
                    || await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count2Section, Field._mva)
                    || await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count3Section, Field._mva)) 
                    ? Field._selected : Field._unselected;
            }
                
            _logger.LogTrace($"isMVA should be: {isMVA.Value}");
        }
        
        // If the section text for any of the 3 counts references a valid MVAR Statute, the isMVAR checkbox should be selected
        if (violationTicket.Fields.TryGetValue(OcrViolationTicket.OffenceIsMVAR, out Field? isMVAR)) {
            // If field neither, it couldn't be read successfully, attempt to use statutes instead.
            if (isMVAR.Value != Field._selected && isMVAR.Value != Field._unselected) { 
                isMVAR.Value = (
                    await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count1Section, Field._mvar)
                    || await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count2Section, Field._mvar)
                    || await IsStatuteValidAsync(violationTicket, OcrViolationTicket.Count3Section, Field._mvar)) 
                    ? Field._selected : Field._unselected;
            }

            _logger.LogTrace($"isMVAR should be: {isMVAR.Value}");
        }
    }

    /// <summary>
    /// Returns true if the given sectionKey exists in the Statutes table and references the given act.
    /// </summary>
    /// <param name="violationTicket"></param>
    /// <param name="sectionKey"></param>
    /// <param name="actCode"></param>
    /// <returns></returns>
    private async Task<bool> IsStatuteValidAsync(OcrViolationTicket violationTicket, string sectionKey, string actCode)
    {
        if (violationTicket.Fields.TryGetValue(sectionKey, out Field? field))
        {
            string? sectionText = field?.Value?.Trim();
            if (!String.IsNullOrEmpty(sectionText))
            {
                sectionText = Regex.Replace(sectionText, @"^\$$", ""); // remove $ if it's the only character.
                Domain.Models.Statute? statute = await _lookupService.GetBySectionAsync(sectionText);
                if (statute is null) {
                    _logger.LogTrace($"Statute not found: {sectionText}");
                    return false;
                }
                else {
                    bool matchesAct = statute?.ActCode == actCode;
                    if (!matchesAct) {
                        _logger.LogTrace($"Statute {sectionText} does not match act code {actCode}");
                    }
                    else {
                        _logger.LogTrace($"Statute found: {actCode} {sectionText}");
                    }
                    return matchesAct;
                }
            }
        }
        return false;
    }

    /// <summary>Applies a set of validation rules to determine if the given violationTicket is valid or not.</summary>
    private static async void ApplyGlobalRules(OcrViolationTicket violationTicket)
    {
        // TCVP-933 A ticket is considered valid iff
        // - TCVP-2559 Ticket Version must not be VT1 (superceded by VT2 and is no longer supported)
        // - Ticket title reads 'VIOLATION TICKET' at top
        // - Ticket number must start with 'A', another alphabetic character, and then 8 digits
        // - Count ACT/REGs must be MVA or MVR as text - all 3 counts must be MVA/R at this time.
        // - If the Date of Service is less than 30 days for handwritten tickets
        List<ValidationRule> rules = new();
        rules.Add(new VersionVT1DisallowedRule(new Field(), violationTicket));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle], _ticketTitleRegex, ValidationMessages.TicketTitleInvalid));
        rules.Add(new FieldMatchesRegexRule(violationTicket.Fields[OcrViolationTicket.ViolationTicketNumber], _violationTicketNumberRegex, ValidationMessages.TicketNumberInvalid));
        if (ViolationTicketVersion.VT1.Equals(violationTicket.TicketVersion))
        {
            // The Act/Regs is only applicable for the old VT1 images
            rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count1ActRegs], 1));
            rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count2ActRegs], 2));
            rules.Add(new CountActRegMustBeMVA(violationTicket.Fields[OcrViolationTicket.Count3ActRegs], 3));
        }
        rules.Add(new DateOfServiceLT30Rule(violationTicket.Fields[OcrViolationTicket.DateOfService]));

        // LRAFED-2650 Only tickets that have MVA or MVAR checked are permitted.
        if (ViolationTicketVersion.VT2.Equals(violationTicket.TicketVersion))
        {
            rules.Add(new DidCommitIsMVA(violationTicket.Fields[OcrViolationTicket.OffenceIsMVA], violationTicket));
        }

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
        if (ViolationTicketVersion.VT1.Equals(violationTicket.TicketVersion))
        {
            // The Act/Regs is only applicable for the old VT1 images
            rules.Add(new FieldIsRequiredRule(violationTicket.Fields[OcrViolationTicket.Count1ActRegs]));
        }
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
