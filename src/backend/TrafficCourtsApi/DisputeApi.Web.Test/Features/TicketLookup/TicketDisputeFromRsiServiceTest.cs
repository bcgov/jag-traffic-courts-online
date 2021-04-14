using AutoFixture;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.TicketLookup;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static DisputeApi.Web.Features.TicketLookup.TicketLookup;

namespace DisputeApi.Web.Test.Features.TicketLookup
{
    public class TicketDisputeFromRsiServiceTest
    {
        [Test, AutoMockAutoData]
        public async Task
            if_rsi_return_response_with_offence_RetrieveTicketDisputeAsync_should_return_response_correctly(           
            RawTicketSearchResponse rawResponse,
            Invoice invoice,
            [Frozen] Mock<IRsiRestApi> rsiApiMock,
            TicketDisputeFromRsiService sut
        )
        {
            var fixture = new Fixture();
            Query query = fixture.Create<Query>();
            rawResponse.Items = new List<Item> { 
                new Item{ SelectedInvoice=new SelectedInvoice{Reference="https://test/EZ020004601"}}
            };
            invoice.ViolationDateTime = "2020-09-18T21:40";
            invoice.InvoiceNumber = "EZ020004601";
            invoice.DiscountAmount = "25.00";
            rsiApiMock.Setup(m => m.GetTicket(It.IsAny<GetTicketParams>(), CancellationToken.None)).Returns(Task.FromResult(rawResponse));
            rsiApiMock.Setup(m => m.GetInvoice(It.Is<string>(m=>m=="EZ020004601"), CancellationToken.None)).Returns(Task.FromResult(invoice));
            var response = await sut.RetrieveTicketDisputeAsync(query.TicketNumber, query.Time, CancellationToken.None);
            Assert.IsInstanceOf<TicketDispute>(response);
            Assert.AreEqual(1, response.Offences.Count);
            Assert.AreEqual("2020-09-18", response.ViolationDate);
            Assert.AreEqual("21:40", response.ViolationTime);
            Assert.AreEqual("2020-10-18", response.Offences[0].DiscountDueDate);
        }

        [Test, AutoMockAutoData]
        public async Task if_rsi_return_noOffences_RetrieveTicketDisputeAsync_should_return_null(
            RawTicketSearchResponse rawResponse,
            [Frozen] Mock<IRsiRestApi> rsiApiMock,
            TicketDisputeFromRsiService sut
        )
        {
            var fixture = new Fixture();
            var query = fixture.Create<Query>();
            rawResponse.Items = null;
            rsiApiMock.Setup(m => m.GetTicket(It.IsAny<GetTicketParams>(), CancellationToken.None)).Returns(Task.FromResult(rawResponse));
            var response = await sut.RetrieveTicketDisputeAsync(query.TicketNumber,query.Time,CancellationToken.None);
            Assert.AreEqual(null, response);
        }
    }
}
