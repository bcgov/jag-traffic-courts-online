using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using Gov.CitizenApi.Models;
using Gov.CitizenApi.Test.Utils;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Tickets.Models
{
    [ExcludeFromCodeCoverage]
    public class TicketModelTest
    {
        [Theory]
        [AutoData]
#pragma warning disable IDE1006 // Naming Styles
        public void can_create_class(Ticket expected)
#pragma warning restore IDE1006 // Naming Styles
        { 
            var actual = PropertyCopy.CopyProperties(expected);

            // to do: check all properties
            Assert.Equal(expected.GivenNames, actual.GivenNames);
            Assert.Equal(expected.Licence, actual.Licence);
        }
    }
}
