using AutoMapper;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.DBModel;
using Gov.CitizenApi.Features.Tickets;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Features.Tickets.Queries;
using Gov.CitizenApi.Models;
using Gov.CitizenApi.Test.Utils;
using Gov.TicketSearch;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Tickets.Queries
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class TicketSearchQueryHandlerTest
    {
        private Mock<ILogger<TicketSearchQueryHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<ITicketSearchClient> _ticketSearchClientMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ITicketsService> _ticketsServiceMock;
        private TicketSearchQueryHandler _sut;

        public TicketSearchQueryHandlerTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketSearchQueryHandler>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _mapperMock = new Mock<IMapper>();
            _ticketSearchClientMock = new Mock<ITicketSearchClient>();
            _ticketsServiceMock = new Mock<ITicketsService>();

            _sut = new TicketSearchQueryHandler(_ticketSearchClientMock.Object, _disputeServiceMock.Object, _mapperMock.Object, _loggerMock.Object, _ticketsServiceMock.Object);
        }

        [Theory]
        [AllowCirculationAutoData]
        public async Task Handle_return_ticketDispute_correctly_when_ticketSearchClient_return_data(
            TicketSearchQuery query,
            TicketSearchResponse clientResponse,
            Dispute dispute,
            TicketDispute ticketDispute,
            Disputant disputant)
        {
            _ticketSearchClientMock.Setup(m => m.TicketsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(clientResponse));
            _disputeServiceMock.Setup(m => m.FindTicketDisputeAsync(It.IsAny<string>())).Returns(Task.FromResult(dispute));
            _mapperMock.Setup(m => m.Map<TicketDispute>(It.IsAny<TicketSearchResponse>())).Returns(ticketDispute);
            _mapperMock.Setup(m => m.Map<Disputant>(It.IsAny<Dispute>())).Returns(disputant);
            _ticketsServiceMock.Setup(m=>m.FindTicketPayments(It.IsAny<string>(),It.IsAny<string>())).Returns((List<Payment>)null);
            var result = await _sut.Handle(query, CancellationToken.None);
            _ticketSearchClientMock.Verify(x => x.TicketsAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
            _disputeServiceMock.Verify(x => x.FindTicketDisputeAsync(clientResponse.ViolationTicketNumber), Times.Once);
            Assert.Equal(ticketDispute.ViolationTicketNumber, result.ViolationTicketNumber);
            Assert.Equal(disputant.DriverLicenseNumber, result.Disputant.DriverLicenseNumber);
        }

        [Theory]
        [AutoMockAutoData]
        public async Task Handle_return_null_when_ticketSearchClient_return_no_data(
            TicketSearchQuery query)
        {
            _ticketSearchClientMock.Setup(m => m.TicketsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TicketSearchResponse>(null));
            var result = await _sut.Handle(query, CancellationToken.None);
            _ticketSearchClientMock.Verify(x => x.TicketsAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
            Assert.True(result == null);
        }


        [Theory]
        [AutoMockAutoData]
        public async Task Handle_return_ticketDispute_when_ticketSearchClient_return_data_disputeService_return_no_data(
            TicketSearchQuery query,
            TicketSearchResponse clientResponse,
            TicketDispute ticketDispute
        )
        {
            _ticketSearchClientMock.Setup(m => m.TicketsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(clientResponse));
            _disputeServiceMock.Setup(m => m.FindTicketDisputeAsync(It.IsAny<string>())).Returns(Task.FromResult<Dispute>(null));
            _mapperMock.Setup(m => m.Map<TicketDispute>(It.IsAny<TicketSearchResponse>())).Returns(ticketDispute);
            _ticketsServiceMock.Setup(m => m.FindTicketPayments(It.IsAny<string>(), It.IsAny<string>())).Returns((List<Payment>)null);
            var result = await _sut.Handle(query, CancellationToken.None);
            _ticketSearchClientMock.Verify(x => x.TicketsAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
            _disputeServiceMock.Verify(x => x.FindTicketDisputeAsync(clientResponse.ViolationTicketNumber), Times.Once);
            Assert.Equal(ticketDispute.ViolationTicketNumber, result.ViolationTicketNumber);
            Assert.Equal(ticketDispute.Disputant.DriverLicenseNumber, result.Disputant.DriverLicenseNumber);
        }
    }
}
