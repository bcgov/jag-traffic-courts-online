using DisputeApi.Web.Features.Tickets.Models;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Tickets.Models
{
    public class TicketModelTest
    {

        [Test]
        public void can_create_class()
        {
            var ticket = new Ticket { Id = "Id", Description ="Description" };
            Assert.DoesNotThrow(() => new Ticket { Id = "Id", Description = "Description" });
            Assert.AreEqual(ticket.Id, "Id");
            Assert.AreEqual(ticket.Description, "Description");
        }
    }
}
