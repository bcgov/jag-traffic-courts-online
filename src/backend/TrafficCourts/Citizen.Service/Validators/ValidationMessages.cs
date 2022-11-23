namespace TrafficCourts.Citizen.Service.Validators;

public static class ValidationMessages
{
    public static readonly string RequiredFieldError = @"{0} is a required field. No value found.";
    public static readonly string FieldIsBlankError = "{0} is blank";
    public static readonly string DriversLicenceNumberError = "Drivers Licence Number is not 7 digits.";
    public static readonly string DriversLicenceProvinceError = "Drivers Licence Province is not 'BC'. Could not validate Drivers Licence Number.";
    public static readonly string TicketTitleInvalid = @"Ticket title must be 'VIOLATION TICKET'.";
    public static readonly string TicketNumberInvalid = @"Violation ticket number must start with an A and be of the form 'AX00000000'.";
    public static readonly string CheckboxInvalid = @"Checkbox '{0}' has an unknown value '{1}'. Expecting 'selected' or 'unselected'.";
    public static readonly string CurrencyInvalid = @"Amount '{0}' is not a valid currency value.";
    public static readonly string MVAMustBeSelectedError = @"MVA must be selected under the 'Did commit the offence(s) indicated' section.";
    public static readonly string MVAMustBeCountValue = @"TCO only supports counts with MVA or MVR as the ACT/REG at this time. Read '{0}' for count {1}.";
    public static readonly string OnlyMVAMustBeSelectedError = @"MVA must be the only selected ACT under the 'Did commit the offence(s) indicated' section.";
    public static readonly string DateFutureInvalid = @"{0} must not be in the future. Read '{1}'.";
    public static readonly string DateOfServiceGT30Days = @"Date of Service '{0}' is more than 30 days ago.";
    public static readonly string TimeInvalid = @"'{0}' is not a valid time.";
    public static readonly string LowConfidenceError = "Too many fields have a low confidence.";
    public static readonly string CountSectionInvalid = @"'{0}' was not found in the list of known MVA Violation Ticket Contraventions.";
}
