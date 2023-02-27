using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Coms.Client;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Services;
using Xunit;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class JJControllerTest
{
    [Fact]
    public async void TestAcceptJJDispute200Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketNumber = "AJ201092461";
        bool checkVTC = false;
        dispute.TicketNumber = ticketNumber;
        List<string> ticketNumbers = new();
        ticketNumbers.Add(ticketNumber);
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AcceptJJDisputeAsync(ticketNumber, It.IsAny<bool>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        
        // Act
        IActionResult? result = await jjDisputeController.AcceptJJDisputeAsync(ticketNumber, checkVTC, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJ200Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to assign it to a JJ, confirm controller updates and assigns the JJ.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        List<string> ticketnumbers = new() {  ticketnumber };
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(ticketnumbers, "Bruce Wayne", It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(ticketnumbers, "Bruce Wayne", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJT400Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with no ticket number to assign it to a JJ, confirm controller returns 400 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(null!, "Bruce Wayne", It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(null!, "Bruce Wayne", CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJT404Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with an invalid ticket number that is not exist in db to assign it to a JJ, confirm controller returns 404 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "invalidTicketNum";
        dispute.TicketNumber = ticketnumber;
        List<string> ticketnumbers = new() { ticketnumber };
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(ticketnumbers, "Bruce Wayne", It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(ticketnumbers, "Bruce Wayne", CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndRequireCourtHearing200Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to set its status to REQUIRE_COURT_HEARING, confirm controller updates the status.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.RequireCourtHearingJJDisputeAsync(ticketnumber, null!, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndRequireCourtHearingJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndRequireCourtHearing400result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with no ticket number to set its status to REQUIRE_COURT_HEARING, confirm controller returns 400 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.RequireCourtHearingJJDisputeAsync(ticketnumber, null!, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndRequireCourtHearingJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndRequireCourtHearing404result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with an invalid ticket number to set its status to REQUIRE_COURT_HEARING, confirm controller returns 404 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "invalidTicketNum";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.RequireCourtHearingJJDisputeAsync(ticketnumber, null!, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndRequireCourtHearingJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndRequireCourtHearing405result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to set its status to REQUIRE_COURT_HEARING that has invalid status and returns 405, confirm controller returns 405 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.RequireCourtHearingJJDisputeAsync(ticketnumber, null!, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status405MethodNotAllowed, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndRequireCourtHearingJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var methodNotAllowedResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status405MethodNotAllowed, methodNotAllowedResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndConfirm200Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to set its status to CONFIRMED, confirm controller updates the status.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.ConfirmJJDisputeAsync(ticketnumber, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndConfirmJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndConfirm400result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with no ticket number to set its status to CONFIRMED, confirm controller returns 400 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.ConfirmJJDisputeAsync(ticketnumber, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndConfirmJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndConfirm404result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with an invalid ticket number to set its status to CONFIRMED, confirm controller returns 404 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "invalidTicketNum";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.ConfirmJJDisputeAsync(ticketnumber, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndConfirmJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateCourtAppearanceAndConfirm405result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to set its status to CONFIRMED that has invalid status and returns 405, confirm controller returns 405 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.ConfirmJJDisputeAsync(ticketnumber, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status405MethodNotAllowed, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.UpdateCourtAppearanceAndConfirmJJDisputeAsync(ticketnumber, CancellationToken.None);

        // Assert
        var methodNotAllowedResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status405MethodNotAllowed, methodNotAllowedResult.StatusCode);
    }

    [Fact]
    public async void TestGetJJDispute200Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJJDisputeAsync(1, ticketnumber, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJJDisputeAsync(1, ticketnumber, false, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dispute, okResult.Value);
    }

    [Fact]
    public async void TestGetJJDispute400Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJJDisputeAsync(1, ticketnumber, false, It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJJDisputeAsync(1, ticketnumber, false, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestGetJJDispute404Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJJDisputeAsync(1, ticketnumber,false, It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJJDisputeAsync(1, ticketnumber, false, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async void TestGetJJDisputeDisputeAssigned409Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJJDisputeAsync(1, ticketnumber, true, It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status409Conflict, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJJDisputeAsync(1, ticketnumber, true, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.Conflict, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("JJ Dispute Already Assigned"));
    }

    [Fact]
    public async void TestGetJJDisputeThrowsObjectManagementServiceException500Result()
    {
        // Arrange
        string ticketnumber = "AJ201092461";
        JJDispute dispute = new()
        {
            TicketNumber = ticketnumber
        };
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJJDisputeAsync(1, ticketnumber, true, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJJDisputeAsync(1, ticketnumber, true, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error Invoking COMS"));
    }
}
