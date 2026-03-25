using TrafficCourts.Domain.Models;

using static TrafficCourts.Domain.Models.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class DidCommitIsMVA : ValidationRule
{
    private readonly OcrViolationTicket _violationTicket;

    public DidCommitIsMVA(Field field, OcrViolationTicket violationTicket) : base(field)
    {
        this._violationTicket = violationTicket;
    }

    public override Task RunAsync(CancellationToken cancellationToken)
    {
        bool isCCLA = this._violationTicket.Fields[OffenceIsCCLA].IsCheckboxSelected() ?? false;
        bool isCTA = this._violationTicket.Fields[OffenceIsCTA].IsCheckboxSelected() ?? false;
        bool isLCLA = this._violationTicket.Fields[OffenceIsLCLA].IsCheckboxSelected() ?? false;
        bool isTCSR = this._violationTicket.Fields[OffenceIsTCSR].IsCheckboxSelected() ?? false;
        bool isWLA = this._violationTicket.Fields[OffenceIsWLA].IsCheckboxSelected() ?? false;
        bool isFVPA = this._violationTicket.Fields[OffenceIsFVPA].IsCheckboxSelected() ?? false;
        bool isOther = this._violationTicket.Fields[OffenceIsOther].IsCheckboxSelected() ?? false;

        // If nothing is selcted we will pass through 
        // If anything other than MVA/MVAR is selected, then we will fail validation. 
        // https://jira.justice.gov.bc.ca/browse/TCVP-3438?focusedCommentId=466537
        if (isCCLA)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsCCLA].TagName, Field.Value));
        }
        if (isCTA)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsCTA].TagName, Field.Value));
        }
        if (isLCLA)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsLCLA].TagName, Field.Value));
        }
        if (isTCSR)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsTCSR].TagName, Field.Value));
        }
        if (isWLA)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsWLA].TagName, Field.Value));
        }
        if (isFVPA)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsFVPA].TagName, Field.Value));
        }
        if (isOther)
        {
            AddValidationError(String.Format(ValidationMessages.OnlyMVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsOther].TagName, Field.Value));
        }

        return Task.CompletedTask;
    }
}
