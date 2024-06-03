using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using TrafficCourts.Arc.Dispute.Service.Controllers;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Controllers
{
    public class TcoDisputeTicketControllerTests
    {

        private readonly ILogger<TcoDisputeTicketController> _mockLogger = Substitute.For<ILogger<TcoDisputeTicketController>>();
        private readonly IMapper _mockMapper = Substitute.For<IMapper>();
        private readonly IArcFileService _mockArcFileService = Substitute.For<IArcFileService>();

        [Fact]
        public async void Test_arcFileRecordList_output_from_mapping_passed_for_ArcFileService_and_returned_result_ok()
        {
            // Arrange
            var disputeTicket = new TcoDisputeTicket
            {
                TicketDetails = new List<TicketCount>
                {
                    { new TicketCount { Act = "MVA" } }
                }
            };

            var disputeTicketController = new TcoDisputeTicketController(_mockMapper, _mockArcFileService, _mockLogger);
            var arcFileRecordList = new List<ArcFileRecord>();

            _mockMapper
                .Map<List<ArcFileRecord>>(Arg.Any<TcoDisputeTicket>())
                .Returns(arcFileRecordList);

            await _mockArcFileService
                .CreateArcFileAsync(Arg.Any<List<ArcFileRecord>>(), Arg.Any<CancellationToken>());

            // Act
            var result = await disputeTicketController.CreateArcFile(disputeTicket, CancellationToken.None);

            // Assert
            //_mockArcFileService.Did
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(arcFileRecordList, okResult.Value);
        }

    }
}
