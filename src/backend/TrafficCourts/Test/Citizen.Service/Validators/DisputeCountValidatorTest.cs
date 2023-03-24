using TrafficCourts.Citizen.Service.Validators;
using Xunit;
using FluentValidation.TestHelper;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using FluentValidation;
using System.Collections.Generic;

namespace TrafficCourts.Test.Citizen.Service.Validators;

public class DisputeCountValidatorTest : ValidatorTest<TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount, DisputeCountValidator>
{
    [Fact]
    public void PleaCode_must_be_not_null()
    {
        TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount model = new();
        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(_ => _.PleaCode)
            .WithErrorCode(NotNullValidator);
    }

    [Theory]
    [InlineData((DisputeCountPleaCode)(-1))]
    [InlineData((DisputeCountPleaCode)4)]
    public void PleaCode_should_produce_validation_error_with_invalid_enum_values(DisputeCountPleaCode? value)
    {
        TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount model = new() { PleaCode = value };
        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(_ => _.PleaCode)
            .WithErrorCode(EnumValidator);
    }

    [Theory]
    [MemberData(nameof(DisputeCountPleaCodeValues))]
    public void PleaCode_should_validate_with_valid_enum_values(DisputeCountPleaCode? value)
    {
        TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount model = new() { PleaCode = value };
        var result = _sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(_ => _.PleaCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(DisputeCountRequestTimeToPay.N)]
    public void RequestTimeToPay_must_be_no_or_null_when_not_guilty_plea(DisputeCountRequestTimeToPay? requestTimeToPay)
    {
        TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount model = new() 
        {
            PleaCode = DisputeCountPleaCode.N,
            RequestTimeToPay = requestTimeToPay
        };

        var result = _sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(_ => _.RequestTimeToPay);
    }

    [Theory]
    [InlineData(DisputeCountRequestTimeToPay.UNKNOWN)]
    [InlineData(DisputeCountRequestTimeToPay.Y)]
    [InlineData((DisputeCountRequestTimeToPay)(-1))]
    [InlineData((DisputeCountRequestTimeToPay)10)]
    public void RequestTimeToPay_must_not_have_other_values_when_not_guilty_plea(DisputeCountRequestTimeToPay? requestTimeToPay)
    {
        TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount model = new()
        {
            PleaCode = DisputeCountPleaCode.N,
            RequestTimeToPay = requestTimeToPay
        };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(_ => _.RequestTimeToPay)
            .WithErrorCode(PredicateValidator);
    }

    public static IEnumerable<object[]> DisputeCountPleaCodeValues()
    {
        foreach (var value in System.Enum.GetValues<DisputeCountPleaCode>())
        {
            yield return new object[] { value };
        }
    }


}
