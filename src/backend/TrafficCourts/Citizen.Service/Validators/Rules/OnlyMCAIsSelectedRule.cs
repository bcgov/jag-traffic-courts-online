using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In the "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' can be selected.
/// If any of the checkboxes in this section are unreadable (namely not "selected" or "unselected") then this rule cannot be verified.
/// </summary>
public class OnlyMCAIsSelectedRule : ValidationRule
{
    private readonly OcrViolationTicket _violationTicket;

    public OnlyMCAIsSelectedRule(OcrViolationTicket violationTicket)
    {
        this._violationTicket = violationTicket;
    }

    public override void Run()
    {
        bool? mva = _violationTicket.GetField(OcrViolationTicket.OffenseIsMVA)?.IsCheckboxSelected();
        bool? mca = _violationTicket.GetField(OcrViolationTicket.OffenseIsMCA)?.IsCheckboxSelected();
        bool? cta = _violationTicket.GetField(OcrViolationTicket.OffenseIsCTA)?.IsCheckboxSelected();
        bool? wla = _violationTicket.GetField(OcrViolationTicket.OffenseIsWLA)?.IsCheckboxSelected();
        bool? faa = _violationTicket.GetField(OcrViolationTicket.OffenseIsFAA)?.IsCheckboxSelected();
        bool? lca = _violationTicket.GetField(OcrViolationTicket.OffenseIsLCA)?.IsCheckboxSelected();
        bool? tcr = _violationTicket.GetField(OcrViolationTicket.OffenseIsTCR)?.IsCheckboxSelected();
        bool? other = _violationTicket.GetField(OcrViolationTicket.OffenseIsOther)?.IsCheckboxSelected();
        
        // If any of the 8 checkboxes is neither "selected" or "unselected", add a validation error.  These values are the only two values from Azure Form Recognizer.
        if (mva is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownMVAValueError);
        }
        if (mca is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownMCAValueError);
        }
        if (cta is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownCTAValueError);
        }
        if (wla is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownWLAValueError);
        }
        if (faa is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownFAAValueError);
        }
        if (lca is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownLCAValueError);
        }
        if (tcr is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownTCRValueError);
        }
        if (other is null)
        {
            ValidationErrors.Add(ValidationMessages.UnknownOtherValueError);
        }

        // If all checkboxes have valid values, ensure only MVA is selected.
        if ((mva is not null) && (mca is not null) && (cta is not null) && (wla is not null) && (faa is not null) && (lca is not null) && (tcr is not null) && (other is not null))
        {
            if (!mva ?? false)
            {
                ValidationErrors.Add(ValidationMessages.MVAMustBeSelectedError);
            }
            if ((mca ?? false) || (cta ?? false) || (wla ?? false) || (faa ?? false) || (lca ?? false) || (tcr ?? false) || (other ?? false))
            {
                ValidationErrors.Add(ValidationMessages.OnlyMVAMustBeSelectedError);
            }
        }
    }
}
