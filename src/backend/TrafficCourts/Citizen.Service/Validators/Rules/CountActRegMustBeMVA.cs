using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In the "Did commit offence(s) indicated, under the following act or its regulations" section, only 'MVA' can be selected.
/// If any of the checkboxes in this section are unreadable (namely not "selected" or "unselected") then this rule cannot be verified.
/// </summary>
public class CountActRegMustBeMVA : ValidationRule
{

    private int _countNum;

    public CountActRegMustBeMVA(Field field, int countNum) : base(field)
    {
        _countNum = countNum;
    }

    public override void Run()
    {
        string? countAct = this.Field.Value;
        if (countAct is not null && !"MVA".Equals(countAct.Replace("\\s+", "").ToUpper()))
        {
            AddValidationError(String.Format(ValidationMessages.MVAMustBeCountValue, countAct, _countNum));
        }
    }
}
