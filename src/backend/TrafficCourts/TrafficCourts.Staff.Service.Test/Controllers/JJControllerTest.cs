using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
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
    public async void TestGetJustinDocument200Result()
    {
        // Arrange
        TicketImageDataJustinDocument justinDocument = new();
        string ticketnumber = "AJ201092461";
        justinDocument.ReportType = TicketImageDataJustinDocumentReportType.TICKET_IMAGE;
        justinDocument.ReportFormat = "pdfa";
        justinDocument.Index = "1";
        justinDocument.ParticipantName = "Roberts, Jon";
        justinDocument.PartId = "2203.03";
        justinDocument.FileData = "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NTYgNTU2CjU1NiAwIDMzMyA1MDAgMjc4IDU1NiA1MDAgNzIyIDUwMCA1MDAgMCAwIDAgMCAwIDM1MAowIDM1MCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMzUwIDAgMzUwCjM1MCAwIDAgMCAwIDM1MF0vRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nL0Jhc2VGb250L0JLR0xLQytBcmlhbE1UL0ZvbnREZXNjcmlwdG9yIDE0IDAgUj4+CmVuZG9iago2IDAgb2JqCjw8L1R5cGUvRm9udC9TdWJ0eXBlL1R5cGUxL0ZpcnN0Q2hhciAzMi9MYXN0Q2hhciAxMjAvV2lkdGhzWzIxMiAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMAowIDUxMyAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAKMCA2MTIgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwCjUzMiAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMAowIDQ4MiAwIDAgMCA1MDEgMjkyIDU1OSAwIDAgMCAwIDIzNiAwIDAgNTQ5CjAgMCAwIDAgMCAwIDAgMCA0NjNdL0VuY29kaW5nL1dpbkFuc2lFbmNvZGluZy9CYXNlRm9udC9CS0dMS0UrTXlyaWFkUHJvLVJlZ3VsYXIvRm9udERlc2NyaXB0b3IgMTYgMCBSPj4KZW5kb2JqCjcgMCBvYmoKPDwvVHlwZS9Gb250L1N1YnR5cGUvVHlwZTEvRmlyc3RDaGFyIDMyL0xhc3RDaGFyIDEyMS9XaWR0aHNbMjc4IDAgMCAwIDAgMCAwIDAgMzMzIDMzMyAwIDAgMjc4IDMzMyAyNzggMjc4CjU1NiA1NTYgNTU2IDU1NiA1NTYgNTU2IDU1NiA1NTYgNTU2IDU1NiAzMzMgMCAwIDAgMCAwCjAgNzIyIDcyMiA3MjIgNzIyIDY2NyA2MTEgNzc4IDcyMiAyNzggNTU2IDcyMiA2MTEgODMzIDcyMiA3NzgKNjY3IDAgNzIyIDY2NyA2MTEgNzIyIDY2NyA5NDQgMCA2NjcgMCAwIDAgMCAwIDAKMCA1NTYgNjExIDU1NiA2MTEgNTU2IDMzMyA2MTEgNjExIDI3OCAwIDU1NiAyNzggODg5IDYxMSA2MTEKNjExIDYxMSAzODkgNTU2IDMzMyA2MTEgNTU2IDc3OCA1NTYgNTU2XS9FbmNvZGluZy9XaW5BbnNpRW5jb2RpbmcvQmFzZUZvbnQvQktHTEtKK0FyaWFsLUJvbGRNVC9Gb250RGVzY3JpcHRvciAxOCAwIFI+PgplbmRvYmoKOCAwIG9iago8PC9UeXBlL0ZvbnQvU3VidHlwZS9UeXBlMS9GaXJzdENoYXIgMzIvTGFzdENoYXIgMTIxL1dpZHRoc1syNzggMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAKMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMAowIDAgMCAwIDAgMCA2MTEgMCAwIDAgMCAwIDAgMCAwIDAKMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMAowIDU1NiAwIDAgNjExIDU1NiAwIDAgMCAyNzggMCAwIDI3OCA4ODkgNjExIDYxMQowIDAgMzg5IDU1NiAzMzMgNjExIDU1NiAwIDAgNTU2XS9FbmNvZGluZy9XaW5BbnNpRW5jb2RpbmcvQmFzZUZvbnQvQktHTEtOK0FyaWFsLUJvbGRJdGFsaWNNVC9Gb250RGVzY3JpcHRvciAyMCAwIFI+PgplbmRvYmoKMiAwIG9iago8PC9UeXBlL1BhZ2UvUGFyZW50IDEyIDAgUi9SZXNvdXJjZXMgNCAwIFIvQ29udGVudHMgMyAwIFI+PgplbmRvYmoKMjIgMCBvYmoKPDwvUy9EPj4KZW5kb2JqCjIzIDAgb2JqCjw8L051bXNbMCAyMiAwIFJdPj4KZW5kb2JqCjEyIDAgb2JqCjw8L1R5cGUvUGFnZXMvS2lkc1syIDAgUl0vQ291bnQgMS9NZWRpYUJveFswIDAgNjEyIDc5Ml0+PgplbmRvYmoKMjQgMCBvYmoKPDwvQ3JlYXRpb25EYXRlKEQ6MjAyMzAyMjgxNDE3NDgtMDgnMDAnKS9Qcm9kdWNlcihBZG9iZSBYTUwgRm9ybSBNb2R1bGUgTGlicmFyeSkvTW9kRGF0ZShEOjIwMjMwMjI4MTQxNzQ4LTA4JzAwJykvQ3JlYXRvcihEZXNpZ25lciA2LjUpL1RpdGxlKGdvdDhNQik+PgplbmRvYmoKMjUgMCBvYmoKPDwvVHlwZS9NZXRhZGF0YS9TdWJ0eXBlL1hNTC9MZW5ndGggMzk4ND4+c3RyZWFtDQo8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/Pgo8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA0LjIuMS1jMDQzIDUyLjM3MjcyOCwgMjAwOS8wMS8xOC0xMzoxODo1MyAgICAgICAgIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIj4KICAgICAgICAgPHhtcDpNZXRhZGF0YURhdGU+MjAyMy0wMi0yOFQxNDoxNzo0Ny0wODowMDwveG1wOk1ldGFkYXRhRGF0ZT4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5EZXNpZ25lciA2LjU8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMjMtMDItMjhUMTQ6MTc6NDgtMDg6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0ZURhdGU+MjAyMy0wMi0yOFQxNDoxNzo0OC0wODowMDwveG1wOkNyZWF0ZURhdGU+CiAgICAgIDwvcmRmOkRlc2NyaXB0aW9uPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczpwZGY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vcGRmLzEuMy8iPgogICAgICAgICA8cGRmOlByb2R1Y2VyPkFkb2JlIFhNTCBGb3JtIE1vZHVsZSBMaWJyYXJ5PC9wZGY6UHJvZHVjZXI+CiAgICAgIDwvcmRmOkRlc2NyaXB0aW9uPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyI+CiAgICAgICAgIDx4bXBNTTpEb2N1bWVudElEPnV1aWQ6NWNiNDBiN2MtNTgwMC00MTUxLThjNzUtNWFiMDUyM2IyMTU2PC94bXBNTTpEb2N1bWVudElEPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD51dWlkOmYxM2RkYTBhLTdmMjQtMmUyYS01MzA1LTNmNGEzMDM1MWUyNzwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgICAgIDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiCiAgICAgICAgICAgIHhtbG5zOnBkZnVhaWQ9Imh0dHA6Ly93d3cuYWlpbS5vcmcvcGRmdWEvbnMvaWQvIj4KICAgICAgICAgPHBkZnVhaWQ6cGFydD4xPC9wZGZ1YWlkOnBhcnQ+CiAgICAgIDwvcmRmOkRlc2NyaXB0aW9uPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iPgogICAgICAgICA8ZGM6Zm9ybWF0PmFwcGxpY2F0aW9uL3BkZjwvZGM6Zm9ybWF0PgogICAgICAgICA8ZGM6dGl0bGU+CiAgICAgICAgICAgIDxyZGY6QWx0PgogICAgICAgICAgICAgICA8cmRmOmxpIHhtbDpsYW5nPSJ4LWRlZmF1bHQiPmdvdDhNQjwvcmRmOmxpPgogICAgICAgICAgICA8L3JkZjpBbHQ+CiAgICAgICAgIDwvZGM6dGl0bGU+CiAgICAgIDwvcmRmOkRlc2NyaXB0aW9uPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczpkZXNjPSJodHRwOi8vbnMuYWRvYmUuY29tL3hmYS9wcm9tb3RlZC1kZXNjLyI+CiAgICAgICAgIDxkZXNjOnZlcnNpb24gcmRmOnBhcnNlVHlwZT0iUmVzb3VyY2UiPgogICAgICAgICAgICA8cmRmOnZhbHVlPjYuMi4wLjIwMTYwMzMxLjEuOTI0MzE2LjkyMTg5MDwvcmRmOnZhbHVlPgogICAgICAgICAgICA8ZGVzYzpyZWY+L3RlbXBsYXRlL3N1YmZvcm1bMV08L2Rlc2M6cmVmPgogICAgICAgICA8L2Rlc2M6dmVyc2lvbj4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAKPD94cGFja2V0IGVuZD0idyI/PgoKZW5kc3RyZWFtCmVuZG9iagoyNiAwIG9iago8PC9UeXBlL0NhdGFsb2cvUGFnZXMgMTIgMCBSL01ldGFkYXRhIDI1IDAgUi9QYWdlTGFiZWxzIDIzIDAgUj4+CmVuZG9iagp4cmVmCjAgMjcKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDAwIDAwMDAwIGYgCjAwMDAwMjM4ODkgMDAwMDAgbiAKMDAwMDAwMDAxNiAwMDAwMCBuIAowMDAwMDA0MTU5IDAwMDAwIG4gCjAwMDAwMjIxNzUgMDAwMDAgbiAKMDAwMDAyMjY5NCAwMDAwMCBuIAowMDAwMDIzMDUzIDAwMDAwIG4gCjAwMDAwMjM1MTcgMDAwMDAgbiAKMDAwMDAwNzAwNyAwMDAwMCBuIAowMDAwMDA3MDgyIDAwMDAwIG4gCjAwMDAwMDQzMDQgMDAwMDAgbiAKMDAwMDAyNDAyNSAwMDAwMCBuIAowMDAwMDA0MzM5IDAwMDAwIG4gCjAwMDAwMDcxNTcgMDAwMDAgbiAKMDAwMDAwNzU3NCAwMDAwMCBuIAowMDAwMDEyOTk3IDAwMDAwIG4gCjAwMDAwMTMyNDMgMDAwMDAgbiAKMDAwMDAxNDUxMCAwMDAwMCBuIAowMDAwMDE0OTIzIDAwMDAwIG4gCjAwMDAwMjAzMTkgMDAwMDAgbiAKMDAwMDAyMDU1NCAwMDAwMCBuIAowMDAwMDIzOTY0IDAwMDAwIG4gCjAwMDAwMjM5ODkgMDAwMDAgbiAKMDAwMDAyNDA5OSAwMDAwMCBuIAowMDAwMDI0MjY3IDAwMDAwIG4gCjAwMDAwMjgzMjggMDAwMDAgbiAKdHJhaWxlcgo8PAovU2l6ZSAyNwovUm9vdCAyNiAwIFIKL0luZm8gMjQgMCBSCi9JRFs8NjQ5NmRlNjM3MzVjOGU2MTU4ZjU2NDQ4ODUzYzRkM2I+PDY0OTZkZTYzNzM1YzhlNjE1OGY1NjQ0ODg1M2M0ZDNiPl0KPj4Kc3RhcnR4cmVmCjI4NDA5CiUlRU9GCg==";
        DocumentType documentType = DocumentType.TICKET_IMAGE;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJustinDocumentAsync(ticketnumber, documentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(justinDocument);
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJustinDocument(ticketnumber, documentType, CancellationToken.None);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
    }



    [Fact]
    public async void TestGetJustinDocument404Result()
    {
        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        DocumentType documentType = DocumentType.NOTICE_OF_DISPUTE;
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();

        jjDisputeService
            .Setup(_ => _.GetJustinDocumentAsync(ticketnumber, documentType, It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.GetJustinDocument(ticketnumber, documentType, CancellationToken.None);

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
