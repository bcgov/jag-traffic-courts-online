using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// For all 3 counts, the ACT/REGs section must be "MVA", "MVR", or blank.
/// </summary>
public class CountActRegMustBeMVA : ValidationRule
{
    private readonly int _countNum;

    public CountActRegMustBeMVA(Field field, int countNum) : base(field)
    {
        _countNum = countNum;
    }

    public override Task RunAsync()
    {
        string? countAct = this.Field.Value;
        if (countAct is not null && 
            (!"MVA".Equals(countAct.Replace("\\s+", "").ToUpper())) && (!"MVR".Equals(countAct.Replace("\\s+", "").ToUpper())))
        {
            AddValidationError(String.Format(ValidationMessages.MVAMustBeCountValue, countAct, _countNum));
        }

        return Task.CompletedTask;
    }
}
