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
using System.Net;

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
        var analyseResponse = new AnalyseHandler.AnalyseResponse(new OcrViolationTicket());
        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse);

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(analyseResponse.OcrViolationTicket, okResult.Value);
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
        OcrViolationTicket violationTicket = new OcrViolationTicket();
        violationTicket.GlobalValidationErrors.Add("Some validation error");
        var analyseResponse = new AnalyseHandler.AnalyseResponse(violationTicket);
        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse); // This response has a null (invalid) OcrViolationTicket

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.True(problemDetails?.Title?.StartsWith("Violation Ticket is not valid"));
        Assert.Equal("Some validation error ", problemDetails?.Detail);
    }
}