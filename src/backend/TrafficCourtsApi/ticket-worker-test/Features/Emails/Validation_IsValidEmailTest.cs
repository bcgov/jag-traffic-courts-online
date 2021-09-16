using Xunit;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// This class tests that the email validation function works as expected.
    /// </summary>
    public class Validation_IsValidEmailTest
    {
        [Theory]
        [MemberData(nameof(TestData.GetValidEmails), MemberType = typeof(TestData))]
        public void IsValidEmail(string email)
        {
            Assert.True(Validation.IsValidEmail(email));
        }

        [Theory]
        [MemberData(nameof(TestData.GetInvalidEmails), MemberType = typeof(TestData))]
        public void IsNotValidEmail(string email)
        {
            Assert.False(Validation.IsValidEmail(email));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NullOrWhiteSpace_IsNotValidEmail(string email)
        {
            Assert.False(Validation.IsValidEmail(email));
        }
    }
}
