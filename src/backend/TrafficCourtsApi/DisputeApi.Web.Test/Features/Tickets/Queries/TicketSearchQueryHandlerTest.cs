﻿using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Features.Tickets.Queries;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Gov.TicketSearch;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common;
using Xunit;

namespace DisputeApi.Web.Test.Features.Tickets.Queries
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class TicketSearchQueryHandlerTest
    {
        private Mock<ILogger<TicketSearchQueryHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<ITicketSearchClient> _ticketSearchClientMock;
        private Mock<IMapper> _mapperMock;
        private TicketSearchQueryHandler _sut;

        public TicketSearchQueryHandlerTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketSearchQueryHandler>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _mapperMock = new Mock<IMapper>();
            _ticketSearchClientMock = new Mock<ITicketSearchClient>();

            _sut = new TicketSearchQueryHandler(_ticketSearchClientMock.Object, _disputeServiceMock.Object, _mapperMock.Object,_loggerMock.Object);
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
            var result = await _sut.Handle(query, CancellationToken.None);
            _ticketSearchClientMock.Verify(x => x.TicketsAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
            _disputeServiceMock.Verify(x => x.FindTicketDisputeAsync(clientResponse.ViolationTicketNumber), Times.Once);
            Assert.Equal(ticketDispute.ViolationTicketNumber, result.ViolationTicketNumber);
            Assert.Equal(disputant.DriverLicense, result.Disputant.DriverLicense);
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
            Assert.True(result==null);
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
            var result = await _sut.Handle(query, CancellationToken.None);
            _ticketSearchClientMock.Verify(x => x.TicketsAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
            _disputeServiceMock.Verify(x => x.FindTicketDisputeAsync(clientResponse.ViolationTicketNumber), Times.Once);
            Assert.Equal(ticketDispute.ViolationTicketNumber, result.ViolationTicketNumber);
            Assert.Equal(ticketDispute.Disputant.DriverLicense, result.Disputant.DriverLicense);
        }
    }
}
