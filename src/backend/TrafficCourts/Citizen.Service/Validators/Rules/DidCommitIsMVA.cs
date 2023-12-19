using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using static TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class DidCommitIsMVA : ValidationRule
{
    private readonly OcrViolationTicket _violationTicket;

    public DidCommitIsMVA(Field field, OcrViolationTicket violationTicket) : base(field)
    {
        this._violationTicket = violationTicket;
    }

    public override Task RunAsync()
    {
        bool isMVA = this._violationTicket.Fields[OffenceIsMVA].IsCheckboxSelected() ?? false;
        bool isMVRA = this._violationTicket.Fields[OffenceIsMVAR].IsCheckboxSelected() ?? false;
        bool isCCLA = this._violationTicket.Fields[OffenceIsCCLA].IsCheckboxSelected() ?? false;
        bool isCTA = this._violationTicket.Fields[OffenceIsCTA].IsCheckboxSelected() ?? false;
        bool isLCLA = this._violationTicket.Fields[OffenceIsLCLA].IsCheckboxSelected() ?? false;
        bool isTCSR = this._violationTicket.Fields[OffenceIsTCSR].IsCheckboxSelected() ?? false;
        bool isWLA = this._violationTicket.Fields[OffenceIsWLA].IsCheckboxSelected() ?? false;
        bool isFVPA = this._violationTicket.Fields[OffenceIsFVPA].IsCheckboxSelected() ?? false;
        bool isOther = this._violationTicket.Fields[OffenceIsOther].IsCheckboxSelected() ?? false;

        // If any Did Commit was selected other than MVA or MVAR, throw an error
        if (!isMVA && !isMVRA) {
            AddValidationError(String.Format(ValidationMessages.MVAMustBeSelectedError, this._violationTicket.Fields[OffenceIsMVA].TagName, Field.Value));
        }
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
