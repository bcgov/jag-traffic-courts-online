using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Disputes.Models
{
    [ExcludeFromCodeCoverage]
    public class DisputeModelTest
    {
        [Theory]
        [AutoData]
        public void can_create_class(DisputeViewModel expected)
        {
            var actual = PropertyCopy.CopyProperties(expected);

            // to do: check all properties
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.InformationCertified, actual.InformationCertified);
        }
    }
}
