namespace TrafficCourts.Citizen.Service.Validators;

public static class ValidationMessages
{
    public static readonly string FieldIsBlankError = "{0} is blank";
    public static readonly string TicketTitleInvalid = @"Ticket title must be 'VIOLATION TICKET'.";
    public static readonly string TicketNumberInvalid = @"Violation ticket number must start with an A and be of the form 'AX00000000'.";
    public static readonly string CheckboxInvalid = @"Checkbox '{0}' has an unknown value '{1}'. Expecting 'selected' or 'unselected'.";
    public static readonly string UnknownMVAValueError = @"Unknown MVA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownMCAValueError = @"Unknown MCA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownCTAValueError = @"Unknown CTA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownWLAValueError = @"Unknown WLA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownFAAValueError = @"Unknown FAA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownLCAValueError = @"Unknown LCA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownTCRValueError = @"Unknown TCR selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    public static readonly string UnknownOtherValueError = @"Unknown Other selection. Could not determine if MVA is the only checkbox selected under the 'Did commit the offence(s) indicated' section.";
    public static readonly string MVAMustBeSelectedError = @"MVA must be selected under the 'Did commit the offence(s) indicated' section.";
    public static readonly string OnlyMVAMustBeSelectedError = @"MVA must be the only selected ACT under the 'Did commit the offence(s) indicated' section.";
    public static readonly string ViolationDateInvalid = @"Violation Date must be a valid date. Read '{0}'.";
    public static readonly string ViolationDateGT30Days = @"Violation Date '{0}' is more than 30 days ago.";
}
