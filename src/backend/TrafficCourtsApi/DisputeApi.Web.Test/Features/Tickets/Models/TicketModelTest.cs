using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Xunit;

namespace DisputeApi.Web.Test.Features.Tickets.Models
{
    [ExcludeFromCodeCoverage]
    public class TicketModelTest
    {
        [Theory]
        [AutoData]
        public void can_create_class(Ticket expected)
        { 
            var actual = PropertyCopy.CopyProperties(expected);

            // to do: check all properties
            Assert.Equal(expected.GivenNames, actual.GivenNames);
            Assert.Equal(expected.Licence, actual.Licence);
        }
    }
}
