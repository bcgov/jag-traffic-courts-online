using DisputeApi.Web.Features.TcoDispute.Models;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TcoDispute.Models
{
    public class DisputeModelTest
    {
        [Test]
        public void can_create_class()
        {
            var dispute = new Dispute
            {
                Id = 1001,
                TicketId = 0002,
                EmailAddress = "jones_234@email.com",
                LawyerPresent = true,
                InterpreterRequired = true,
                InterpreterLanguage = "Spanish",
                CallWitness = false,
                CertifyCorrect = true,
                StatusCode = "SUBM"
            };
            Assert.AreEqual(dispute.EmailAddress, "jones_234@email.com");
            Assert.AreEqual(dispute.CertifyCorrect, true);
        }
    }
}
