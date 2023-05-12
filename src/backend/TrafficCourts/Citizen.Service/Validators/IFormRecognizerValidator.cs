using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators;

public interface IFormRecognizerValidator
{
    /// <summary>
    ///    Validates the Violation Ticket, updating the global and field-specific confidence scores and validation results.
    ///    ie. A valid handwritten Violation Ticket must have the title "Violation Ticket"
    ///    ie. A valid ticket number must have 2 characters (starting with A) followed by 8 digits
    ///    ie. Count ticket amounts must be numeric
    ///    ie. Drivers Licence must be 7 digits
    /// </summary>
    public Task ValidateViolationTicketAsync(OcrViolationTicket violationTicket);
}
