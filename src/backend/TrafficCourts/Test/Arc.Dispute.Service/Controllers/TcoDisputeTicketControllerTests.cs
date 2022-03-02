using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Controllers;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Controllers
{
    public class TcoDisputeTicketControllerTests
    {
        private readonly Mock<ILogger<TcoDisputeTicketController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IArcFileService> _mockArcFileService;

        public TcoDisputeTicketControllerTests()
        {
            _mockLogger = new Mock<ILogger<TcoDisputeTicketController>>();
            _mockMapper = new Mock<IMapper>();
            _mockArcFileService = new Mock<IArcFileService>();
        }

        [Fact]
        public async void Test_arcFileRecordList_output_from_mapping_passed_for_ArcFileService_and_returned_result_ok()
        {
            // Arrange
            var mockTcoDisputeTicket = new Mock<TcoDisputeTicket>();
            var disputeTicketController = new TcoDisputeTicketController(_mockMapper.Object, _mockArcFileService.Object);
            var arcFileRecordList = new List<ArcFileRecord>();
            _mockMapper
                .Setup(_ => _.Map<List<ArcFileRecord>>(It.IsAny<TcoDisputeTicket>()))
                .Returns(arcFileRecordList);

            _mockArcFileService
                .Setup(_ => _.CreateArcFile(It.Is<List<ArcFileRecord>>((_) => _ == arcFileRecordList)));

            // Act
            var result = await disputeTicketController.CreateArcFile(mockTcoDisputeTicket.Object);

            // Assert
            _mockArcFileService.VerifyAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(arcFileRecordList, okResult.Value);
        }

    }
}
