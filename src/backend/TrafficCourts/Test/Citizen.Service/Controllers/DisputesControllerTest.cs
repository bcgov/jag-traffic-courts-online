using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Controllers;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Dispute;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Controllers
{
    public class DisputesControllerTest
    {
        [Fact]
        public async void TestCreateDisputeOkResult()
        {
            // Arrange
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var disputeController = new DisputesController(mockMediator.Object, mockLogger.Object);
            var request = new Create.Request(mockTicketDispute.Object);
            var createResponse = new Create.Response(true);
            mockMediator
                .Setup(_ => _.Send<Create.Response>(It.IsAny<Create.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createResponse);

            // Act
            var result = await disputeController.CreateAsync(mockTicketDispute.Object, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}
