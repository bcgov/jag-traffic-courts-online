using Moq;
using System;
using Xunit;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Tests to ensure the IsAllowed parameter validation is working correctly.
    /// Derived classes assume the base class has validated all preconditions.
    /// </summary>
    public class EmailFilterTest
    {
        [Fact]
        public void DoesNotAcceptNull()
        {
            EmailFilter sut = new Mock<EmailFilter>().Object;
            var actual = Assert.Throws<ArgumentNullException>(() => sut.IsAllowed(null));
            Assert.Equal("email", actual.ParamName);
        }

        [Fact]
        public void DoesNotAcceptEmptyString()
        {
            EmailFilter sut = new Mock<EmailFilter>().Object;
            var actual = Assert.Throws<ArgumentException>(() => sut.IsAllowed(string.Empty));
            Assert.Equal("email", actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(TestData.GetInvalidEmails), MemberType = typeof(TestData))]
        public void DoesNotAcceptInvalidEmail(string email)
        {
            EmailFilter sut = new Mock<EmailFilter>().Object;
            var actual = Assert.Throws<FormatException>(() => sut.IsAllowed(email));
            Assert.Equal("Email address is not a valid email address", actual.Message);
        }
    }
}
