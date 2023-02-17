using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In the "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' can be selected.
/// If any of the checkboxes in this section are unreadable (namely not ":selected:" or ":unselected:") then this rule cannot be verified.
/// </summary>
public class OnlyMVAIsSelectedRule : ValidationRule
{
    private readonly OcrViolationTicket _violationTicket;

    public OnlyMVAIsSelectedRule(Field field, OcrViolationTicket violationTicket) : base(field)
    {
        this._violationTicket = violationTicket;
    }

    public override Task RunAsync()
    {
        bool? mva = _violationTicket.Fields[OcrViolationTicket.OffenceIsMVA].IsCheckboxSelected();
        bool? mca = _violationTicket.Fields[OcrViolationTicket.OffenceIsMCA].IsCheckboxSelected();
        bool? cta = _violationTicket.Fields[OcrViolationTicket.OffenceIsCTA].IsCheckboxSelected();
        bool? wla = _violationTicket.Fields[OcrViolationTicket.OffenceIsWLA].IsCheckboxSelected();
        bool? faa = _violationTicket.Fields[OcrViolationTicket.OffenceIsFAA].IsCheckboxSelected();
        bool? lca = _violationTicket.Fields[OcrViolationTicket.OffenceIsLCA].IsCheckboxSelected();
        bool? tcr = _violationTicket.Fields[OcrViolationTicket.OffenceIsTCR].IsCheckboxSelected();
        bool? other = _violationTicket.Fields[OcrViolationTicket.OffenceIsOther].IsCheckboxSelected();

        // TCVP-1645 MVA no longer needs to be selected, so long as nothing else is selected.
        // code removed (see git history to restore)

        if ((mca ?? false) || (cta ?? false) || (wla ?? false) || (faa ?? false) || (lca ?? false) || (tcr ?? false) || (other ?? false))
        {
            AddValidationError(ValidationMessages.OnlyMVAMustBeSelectedError);
        }

        return Task.CompletedTask;
    }
}
