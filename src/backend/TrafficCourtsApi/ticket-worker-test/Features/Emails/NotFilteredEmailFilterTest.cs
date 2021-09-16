using Xunit;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Validates that any valid email address is allowed.
    /// Note we dont need to test parameter validation 
    /// because the base class does that.
    /// </summary>
    public class NotFilteredEmailFilterTest
    {
        [Theory]
        [MemberData(nameof(TestData.GetValidEmails), MemberType = typeof(TestData))]
        public void AllValidEmailsAreAllowed(string email)
        {
            var sut = new NotFilteredEmailFilter();
            Assert.True(sut.IsAllowed(email));
        }
    }
}
