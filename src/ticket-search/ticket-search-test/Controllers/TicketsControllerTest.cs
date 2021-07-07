using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Gov.TicketSearch.Controllers;
using Gov.TicketSearch.Models;
using Gov.TicketSearch.Services;
using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gov.TicketSearch.Test.Controllers
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class TicketsControllerTest
    {
        private Mock<ILogger<TicketsController>> _loggerMock;
        private Mock<ITicketsService> _ticketsServiceMock;

        public TicketsControllerTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
            _ticketsServiceMock = new Mock<ITicketsService>();
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsController(null, _ticketsServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, null));
        }

 

        [Theory]
        [AutoData]
        public async Task Get_return_response_with_OK(TicketSearchRequest query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
            RawTicketSearchResponse response = CreateTestRawTicketResponse();
            _ticketsServiceMock.Setup(m => m.SearchTicketAsync(It.IsAny<string>(),It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult(response));
            var result = (OkObjectResult)await sut.Get(query);
            Assert.IsAssignableFrom<TicketSearchResponse>(result.Value);
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_null_with_NoContent(TicketSearchRequest query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
            _ticketsServiceMock.Setup(m => m.SearchTicketAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult((RawTicketSearchResponse)null));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (NoContentResult)await sut.Get(query);
            Assert.Equal(204, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_500_with_wrong_data(TicketSearchRequest query)
        {
            RawTicketSearchResponse testResponse = CreateTestRawTicketResponse();
            testResponse.Items[0].SelectedInvoice.Invoice.ViolationDateTime = "wrong date format";
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
            _ticketsServiceMock.Setup(m => m.SearchTicketAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult((RawTicketSearchResponse)testResponse));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = await sut.Get(query);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        private RawTicketSearchResponse CreateTestRawTicketResponse()
        {
            Invoice invoice = new Invoice
            {
                AccountNumber = "EZ020004602",
                DiscountAmount="10",
                AmountDue=23.09M,
                InvoiceNumber="39014801384",
                InvoiceType="type",
                OffenceDescription="offence Description",
                PartyName="n/a",
                PartyNumber="n/a",
                TicketedAmount=109.89M,
                ViolationDateTime= "2020-09-18T09:54"
            };
            RawTicketSearchResponse testResponse = new RawTicketSearchResponse
            {
                Items = new List<Item>
                {
                    new Item { SelectedInvoice = new SelectedInvoice { Invoice = invoice } },

                }
            };
            return testResponse;
        }
    }
}
