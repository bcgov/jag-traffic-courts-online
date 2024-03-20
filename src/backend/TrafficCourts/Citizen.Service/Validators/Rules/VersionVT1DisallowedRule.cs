using TrafficCourts.Domain.Models;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// Violation Tickets that were originally used 2022-04 (aka VT1) are no longer supported.
/// @see TCVP-2559
/// </summary>
public class VersionVT1DisallowedRule : ValidationRule {

    private readonly OcrViolationTicket _violationTicket;

    public VersionVT1DisallowedRule(Field field, OcrViolationTicket violationTicket) : base(field)
    {
        this._violationTicket = violationTicket;
    }

    public override Task RunAsync()
    {
        if (ViolationTicketVersion.VT1 == _violationTicket.TicketVersion) {
            _violationTicket.GlobalValidationErrors.Add(ValidationMessages.TicketVersionInvalid);
        }

        return Task.CompletedTask;
    }
}
