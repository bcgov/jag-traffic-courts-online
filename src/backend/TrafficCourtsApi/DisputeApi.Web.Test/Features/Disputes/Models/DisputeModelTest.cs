using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Test.Utils;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Disputes.Models
{
    [ExcludeFromCodeCoverage]
    public class DisputeModelTest
    {
        [Theory]
        [AutoData]
        public void can_create_class(Dispute expected)
        {
            var actual = PropertyCopy.CopyProperties(expected);

            // to do: check all properties
            Assert.AreEqual(expected.DisputantEmailAddress, actual.DisputantEmailAddress);
            Assert.AreEqual(expected.InformationCertified, actual.InformationCertified);
        }
    }
}
