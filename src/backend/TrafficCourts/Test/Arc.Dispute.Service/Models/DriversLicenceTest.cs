using TrafficCourts.Arc.Dispute.Service.Models;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Models
{
    public class DriversLicenceTest
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("1111111", "011111112")]
        [InlineData("01111111", "011111112")]
        [InlineData("1234567", "012345678")]
        [InlineData("01234567", "012345678")]
        [InlineData("1690028", "016900280")]
        public void WithCheckDigit(string input, string expected)
        {
            string actual = DriversLicence.WithCheckDigit(input);

            Assert.Equal(expected, actual);
        }
    }
}
