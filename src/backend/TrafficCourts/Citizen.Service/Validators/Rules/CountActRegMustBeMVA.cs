using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// For all 3 counts, the ACT/REGs section must be "MVA" or blank.
/// </summary>
public class CountActRegMustBeMVA : ValidationRule
{

    private int _countNum;

    public CountActRegMustBeMVA(Field field, int countNum) : base(field)
    {
        _countNum = countNum;
    }

    public override Task RunAsync()
    {
        string? countAct = this.Field.Value;
        if (countAct is not null && !"MVA".Equals(countAct.Replace("\\s+", "").ToUpper()))
        {
            AddValidationError(String.Format(ValidationMessages.MVAMustBeCountValue, countAct, _countNum));
        }

        return Task.CompletedTask;
    }
}
