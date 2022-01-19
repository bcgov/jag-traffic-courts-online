using Xunit;
using MediatR;
using Moq;
using TrafficCourts.Citizen.Service.Features.Tickets;
using Microsoft.AspNetCore.Http;
using System.Threading;
using TrafficCourts.Citizen.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Citizen.Service.Models.Tickets;
using Microsoft.Extensions.Logging;

namespace TrafficCourts.Test.Citizen.Service.Controllers;

public class TicketsControllerTests
{
    [Fact]
    public async void TestAnalyseOkResult()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var ticketController = new TicketsController(mockMediator.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        var analyseResponse = new AnalyseHandler.AnalyseResponse();
        analyseResponse.OcrViolationTicket = new OcrViolationTicket();
        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse);

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(analyseResponse, okResult.Value);
    }

    [Fact]
    public async void TestAnalyseBadRequest()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var ticketController = new TicketsController(mockMediator.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        var analyseResponse = new AnalyseHandler.AnalyseResponse();
        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse); // This response has a null (invalid) OcrViolationTicket

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
    }
}