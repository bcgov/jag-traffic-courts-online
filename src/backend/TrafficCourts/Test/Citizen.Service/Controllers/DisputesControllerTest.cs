﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
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
            mockTicketDispute.Object.FineReductionReason = "fine reduction reason";
            mockTicketDispute.Object.TimeToPayReason = "time to pay reason";
            mockTicketDispute.Object.DisputedCounts.Add(Mock.Of<DisputedCount>());
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var disputeController = new DisputesController(mockMediator.Object, mockLogger.Object);
            var request = new Create.Request(mockTicketDispute.Object);
            var createResponse = new Create.Response();

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
