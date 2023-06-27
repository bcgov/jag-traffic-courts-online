using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Validators;
using Xunit;
using FluentValidation.TestHelper;
using FluentValidation;
using System.Collections.Generic;

namespace TrafficCourts.Test.Citizen.Service.Validators;

public class NoticeOfDisputeValidatorTest : ValidatorTest<NoticeOfDispute, NoticeOfDisputeValidator>
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TicketNumber_must_be_not_null_or_empty(string? value)
    {
        NoticeOfDispute model = new() { TicketNumber = value };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(_ => _.TicketNumber)
            .WithErrorCode(NotEmptyValidator);
    }

    [Fact]
    public void TicketNumber_must_have_max_length_of_12()
    {
        NoticeOfDispute model = new() { TicketNumber = new string('A', 13) };

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(_ => _.TicketNumber)
            .WithErrorCode(MaximumLengthValidator);
    }
}
