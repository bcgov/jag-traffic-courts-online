using AutoFixture;
using AutoFixture.Xunit2;
using Gov.TicketSearch.Services.RsiServices;
using Gov.TicketSearch.Services.RsiServices.Models;
using Gov.TicketSearch.Test;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ticket_search_test.Services
{
    public class RsiTicketsServiceTest
    {
        [Theory]
        [AutoMockAutoData]
        public async Task SearchTicketAsync_should_return_response_correctly(
                RawTicketSearchResponse rawResponse,
                Invoice invoice,
                [Frozen] Mock<IRsiRestApi> rsiApiMock,
                RsiTicketsService sut
        )
        {
            rawResponse.Items = new List<Item> {
                new Item{ SelectedInvoice=new SelectedInvoice{Reference="https://test/EZ020004601"}}
            };
            invoice.ViolationDateTime = "2020-09-18T21:40";
            invoice.InvoiceNumber = "EZ020004601";
            invoice.DiscountAmount = "25.00";
            rsiApiMock.Setup(m => m.GetTicket(It.IsAny<GetTicketParams>(), CancellationToken.None)).Returns(Task.FromResult(rawResponse));
            rsiApiMock.Setup(m => m.GetInvoice(It.Is<string>(m => m == "EZ020004601"), CancellationToken.None)).Returns(Task.FromResult(invoice));
            var response = await sut.SearchTicketAsync("EZ02000460", "21:40", CancellationToken.None);
            Assert.IsAssignableFrom<RawTicketSearchResponse>(response);
            Assert.Single(response.Items);
            Assert.Equal("2020-09-18T21:40", response.Items[0].SelectedInvoice.Invoice.ViolationDateTime);
        }

        [Theory]
        [AutoMockAutoData]
        public async Task SearchTicketAsync_should_return_null_if_no_invoice(

            RawTicketSearchResponse rawResponse,
            [Frozen] Mock<IRsiRestApi> rsiApiMock,
            RsiTicketsService sut
        )
        {
            rawResponse.Items = null;
            rsiApiMock.Setup(m => m.GetTicket(It.IsAny<GetTicketParams>(), CancellationToken.None)).Returns(Task.FromResult(rawResponse));
            var response = await sut.SearchTicketAsync("EZ02000460", "21:40", CancellationToken.None);
            Assert.Null(response);
        }
    }
}
