using DisputeApi.Web.Features.TicketService.Models;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TicketService.Models
{
    public class TicketModelTest
    {

        [Test]
        public void can_create_class()
        {
            var ticket = new Ticket {  TicketNumber = 11234,
                    Name = "John Doe",
                    DateOfIssue = "11-12-2002",
                    TimeOfIssue = "12:23",
                    DriversLicence = "L2323G7" };
            Assert.DoesNotThrow(() => new Ticket {  TicketNumber = 11234,
                    Name = "John Doe",
                    DateOfIssue = "11-12-2002",
                    TimeOfIssue = "12:23",
                    DriversLicence = "L2323G7" });
            Assert.AreEqual(ticket.Name, "John Doe");
            Assert.AreEqual(ticket.DriversLicence, "L2323G7");
        }
    }
}
