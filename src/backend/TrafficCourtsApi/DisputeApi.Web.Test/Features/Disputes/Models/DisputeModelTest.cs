using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Test.Utils;
using Xunit;

namespace DisputeApi.Web.Test.Features.Disputes.Models
{
    [ExcludeFromCodeCoverage]
    public class DisputeModelTest
    {
        [Fact]
        public void can_create_class()
        {
            var expected = new Dispute { DisputantEmailAddress = "test@test.com", InformationCertified = true };
            var actual = PropertyCopy.CopyProperties(expected);

            // to do: check all properties
            Assert.Equal(expected.DisputantEmailAddress, actual.DisputantEmailAddress);
            Assert.Equal(expected.InformationCertified, actual.InformationCertified);
        }
    }
}
