using System;
using Xunit;

namespace Gov.TicketWorker.Features.Emails
{
    public class DefaultEmailFilterTest
    {
        [Fact]
        public void DoesNotAcceptNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultEmailFilter(null));
        }

        [Theory]
        [MemberData(nameof(TestData.GetValidEmails), MemberType = typeof(TestData))]
        public void IsValidEmail(string email)
        {
            var sut = new DefaultEmailFilter(Array.Empty<string>());
            Assert.False(sut.IsAllowed(email));
        }

        [Fact]
        public void EmailsShouldBeMatchedIgnoringCase()
        {
            var allowed = "bob@example.com";
            var sut = new DefaultEmailFilter(new string[] { allowed });
            Assert.True(sut.IsAllowed(allowed.ToUpper()));
        }
    }
}
