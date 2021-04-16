using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using NUnit.Framework;

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
            Assert.AreEqual(expected.GivenNames, actual.GivenNames);
            Assert.AreEqual(expected.Licence, actual.Licence);
        }
    }
}
