namespace TrafficCourts.Citizen.Service.Validators;

public static class ValidationMessages
{
    internal static readonly string FieldIsBlankError = "{0} is blank";
    internal static readonly string TicketTitleInvalid = @"Ticket title must be 'VIOLATION TICKET'.";
    internal static readonly string TicketNumberInvalid = @"Violation ticket number must start with an A and be of the form 'AX00000000'.";
    internal static readonly string UnknownMVAValueError = @"Unknown MVA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownMCAValueError = @"Unknown MCA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownCTAValueError = @"Unknown CTA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownWLAValueError = @"Unknown WLA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownFAAValueError = @"Unknown FAA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownLCAValueError = @"Unknown LCA selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownTCRValueError = @"Unknown TCR selection. Could not determine if MVA is the only checkbox selected under 'Did commit the offence(s) indicated' section.";
    internal static readonly string UnknownOtherValueError = @"Unknown Other selection. Could not determine if MVA is the only checkbox selected under the 'Did commit the offence(s) indicated' section.";
    internal static readonly string MVAMustBeSelectedError = @"MVA must be selected under the 'Did commit the offence(s) indicated' section.";
    internal static readonly string OnlyMVAMustBeSelectedError = @"MVA must be the only selected ACT under the 'Did commit the offence(s) indicated' section.";
    internal static readonly string ViolationDateInvalid = @"Violation Date must be a valid date.";
    internal static readonly string ViolationDateGT30Days = @"Violation Date is more than 30 days ago.";
}
