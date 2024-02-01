using Xunit;
using MediatR;
using Moq;
using TrafficCourts.Citizen.Service.Features.Tickets;
using Microsoft.AspNetCore.Http;
using System.Threading;
using TrafficCourts.Citizen.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using MassTransit;
using System.Collections.Generic;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;

using SearchRequest = TrafficCourts.Citizen.Service.Features.Tickets.Search.Request;
using SearchResponse = TrafficCourts.Citizen.Service.Features.Tickets.Search.Response;

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
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        OcrViolationTicket ticket = new OcrViolationTicket();
        Dictionary<string, Field> fields = new Dictionary<string, Field>();
        Field field = new();
        string ticketNumber = "AC63378564";
        field.JsonName = "ticket_number";
        field.Value = ticketNumber;
        fields.Add(OcrViolationTicket.ViolationTicketNumber, field);
        ticket.Fields = fields;
        var analyseResponse = new AnalyseHandler.AnalyseResponse(ticket);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse);

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None, false, false);

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
        var mockBus = new Mock<IBus>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        OcrViolationTicket violationTicket = new();
        violationTicket.GlobalValidationErrors.Add("Some validation error");
        var analyseResponse = new AnalyseHandler.AnalyseResponse(violationTicket);
        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse); // This response has a null (invalid) OcrViolationTicket

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None, false, false);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.StartsWith("Violation Ticket is not valid"));
    }

    [Fact]
    public async void TestAnalyseBadRequest_DisputeSubmittedBefore()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var mockBus = new Mock<IBus>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        OcrViolationTicket violationTicket = new();
        Dictionary<string, Field> fields = new Dictionary<string, Field>();
        List<string> errors = new List<string>();
        Field field = new();
        string ticketNumber = "AC63378564";
        field.JsonName = "ticket_number";
        field.Value = ticketNumber;
        fields.Add(OcrViolationTicket.ViolationTicketNumber, field);
        violationTicket.Fields = fields;
        violationTicket.GlobalValidationErrors = errors;
        var analyseResponse = new AnalyseHandler.AnalyseResponse(violationTicket);

        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None, false, false);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorMsg = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.StartsWith("A dispute has already been submitted", errorMsg);
    }

    [Fact]
    public async void TestAnalyseInternalError_DisputeSearchFailed()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var mockBus = new Mock<IBus>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);
        OcrViolationTicket violationTicket = new();
        Dictionary<string, Field> fields = new Dictionary<string, Field>();
        List<string> errors = new List<string>();
        Field field = new();
        string ticketNumber = "AC63378564";
        field.JsonName = "ticket_number";
        field.Value = ticketNumber;
        fields.Add(OcrViolationTicket.ViolationTicketNumber, field);
        violationTicket.Fields = fields;
        violationTicket.GlobalValidationErrors = errors;
        var analyseResponse = new AnalyseHandler.AnalyseResponse(violationTicket);

        DisputeSearchFailedException exception = new DisputeSearchFailedException(ticketNumber);

        mockMediator
            .Setup(_ => _.Send<AnalyseHandler.AnalyseResponse>(It.IsAny<AnalyseHandler.AnalyseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analyseResponse);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(exception);

        // Act
        var result = await ticketController.AnalyseAsync(mockImage.Object, CancellationToken.None, false, false);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error searching dispute"));
    }

    [Fact]
    public async void TestSearchOkResult()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        string ticketNumber = "AC63378564";
        string time = "12:15";
        TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket violationTicket = new TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket();
        violationTicket.TicketNumber = ticketNumber;
        SearchResponse response = new(violationTicket);

        mockMediator
            .Setup(_ => _.Send<SearchResponse>(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act
        var result = await ticketController.SearchAsync(ticketNumber, time, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(violationTicket, okResult.Value);
    }

    [Fact]
    public async void TestSearchBadRequest_DisputeSubmittedBefore()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        string ticketNumber = "AC63378564";
        string time = "12:15";
        TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket violationTicket = new TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket();
        violationTicket.TicketNumber = ticketNumber;
        SearchResponse response = new(violationTicket);

        mockMediator
            .Setup(_ => _.Send<SearchResponse>(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        var result = await ticketController.SearchAsync(ticketNumber, time, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorMsg = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.StartsWith("A dispute has already been submitted", errorMsg);
    }

    [Fact]
    public async void TestSearchInternalError_DisputeSearchFailed()
    {
        // Arrange
        var mockImage = new Mock<IFormFile>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<TicketsController>>();
        var mockTicketSearchService = new Mock<ITicketSearchService>();
        var ticketController = new TicketsController(mockMediator.Object, mockTicketSearchService.Object, mockLogger.Object);
        string ticketNumber = "AC63378564";
        string time = "12:15";
        TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket violationTicket = new TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket();
        violationTicket.TicketNumber = ticketNumber;
        SearchResponse response = new(violationTicket);

        DisputeSearchFailedException exception = new DisputeSearchFailedException(ticketNumber);

        mockMediator
            .Setup(_ => _.Send<SearchResponse>(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockTicketSearchService
                 .Setup(_ => _.IsDisputeSubmittedBefore(ticketNumber, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(exception);

        // Act
        var result = await ticketController.SearchAsync(ticketNumber, time, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error searching dispute"));
    }
}