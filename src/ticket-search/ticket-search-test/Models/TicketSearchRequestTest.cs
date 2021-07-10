using Gov.TicketSearch.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Gov.TicketSearch.Test.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class TicketSearchRequestTest
    {
        private const string ValidTicketNumber = "AA000000";

        [Theory]
        [MemberData(nameof(GetValidTimes))]
#pragma warning disable IDE1006 // Naming Styles
        public void should_accept_valid_time_of_day(int hour, int minute)
#pragma warning restore IDE1006 // Naming Styles
        {
            TicketSearchRequest sut = new TicketSearchRequest();
            sut.TicketNumber = ValidTicketNumber;
            sut.Time = $"{hour:D2}:{minute:D2}";
            var validationResults = ValidateModel(sut);

            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(-1, 0, "hour before 0")]
        [InlineData(24, 0, "hour after 23")]
        [InlineData(0, -1, "minute before 0")]
        [InlineData(0, 60, "minute after 59")]
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void should_not_accept_invalid_time_of_day(int hour, int minute, string testCase)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE1006 // Naming Styles
        {
            TicketSearchRequest sut = new TicketSearchRequest();
            sut.TicketNumber = ValidTicketNumber;
            sut.Time = $"{hour:D2}:{minute:D2}";
            var validationResults = ValidateModel(sut);

            Assert.Single(validationResults);
        }

        [Theory]
        [InlineData("123456", "does not start with two letters")]
        [InlineData("A123456", "does not start with two letters")]
        [InlineData("ab123456", "does not start with two uppercase letters")]
        [InlineData("AB12345", "does not have at least 6 digits after letters")]
        [InlineData("AB 123456", "contains invalid character")]
        [InlineData("AB-123456", "contains invalid character")]
        [InlineData("AB_123456", "contains invalid character")]
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void should_not_accept_invalid_ticket_number(string value, string testCase)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE1006 // Naming Styles
        {
            TicketSearchRequest sut = new TicketSearchRequest();
            sut.TicketNumber = value;
            sut.Time = $"00:00";
            var validationResults = ValidateModel(sut);

            Assert.Single(validationResults);
        }

        [Theory]
        [MemberData(nameof(GetValidTimes))]
#pragma warning disable IDE1006 // Naming Styles
        public void should_not_accept_time_of_day_missing_colon(int hour, int minute)
#pragma warning restore IDE1006 // Naming Styles
        {
            TicketSearchRequest sut = new TicketSearchRequest();
            sut.TicketNumber = ValidTicketNumber;
            sut.Time = $"{hour:D2}{minute:D2}";
            var validationResults = ValidateModel(sut);

            Assert.Single(validationResults);
        }

        public static IEnumerable<object[]> GetValidTimes()
        {
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute++)
                {
                    yield return new object[] { hour, minute };
                }
            }
        }


        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
