using DisputeApi.Web.Features.TicketService.Models;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TicketService.Models
{
    public class TicketModelTest
    {

        [Test]
        public void can_create_class()
        {
            var ticket = new Ticket
            {
                Id = 1001,
                UserId = "User123",
                ViolationTicketNumber = "GX87878888",
                ViolationDate = "11-12-2002 12:23",
                SurName = "Smith",
                GivenNames = "John",
                Mailing = "Mailing",
                Postal = "V0W0A0",
                City = "Victoria",
                Province = "BC",
                Licence = "L2323G7",
                ProvLicense = "L34343G64",
                HomePhone = "2434332233",
                WorkPhone = "3345553344",
                Birthdate = "12-12-2002"
            };
            Assert.AreEqual(ticket.GivenNames, "John");
            Assert.AreEqual(ticket.Licence, "L2323G7");
        }
    }
}
