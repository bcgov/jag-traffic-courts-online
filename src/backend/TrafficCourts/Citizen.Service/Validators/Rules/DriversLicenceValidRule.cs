using System.Text.RegularExpressions;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>Validates the Driver's Licence Number is valid, specifically the Driver's Licence Province must be 'BC' and the licence number must have exactly 7 digits.</summary>
public class DriversLicenceValidRule : ValidationRule
{

    private static readonly string DriverLicenceProvinceRegex = @"^BC$";
    private static readonly string DriverLicenceNumberRegex = @"^\d{7}$";

    private readonly OcrViolationTicket _violationTicket;

    public DriversLicenceValidRule(Field field, OcrViolationTicket violationTicket) : base(field)
    {
        this._violationTicket = violationTicket;
    }

    public override Task RunAsync()
    {
        // TCVP-1004
        // - if Driver's Licence province/state = BC and Driver's Licence Number != 7 then flag for staff review
        // - if Driver's Licence province/state != BC flag for staff review
        // - if Driver's Licence province/state is blank flag for staff review
        // - if Driver's Licence Number is blank flag for staff review
        Field province = _violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince];
        bool provMatches = true;
        if (province.Value is not null && !Regex.IsMatch(province.Value, DriverLicenceProvinceRegex))
        {
            province.ValidationErrors.Add(ValidationMessages.DriversLicenceProvinceError);
            provMatches = false;
        }
        
        if (Field.Value is not null && provMatches && !Regex.IsMatch(Field.Value, DriverLicenceNumberRegex))
        {
            AddValidationError(ValidationMessages.DriversLicenceNumberError);
        }

        return Task.CompletedTask;
    }
}
