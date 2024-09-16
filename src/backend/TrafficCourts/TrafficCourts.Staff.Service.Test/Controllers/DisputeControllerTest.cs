﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using TrafficCourts.Domain.Models;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Services;
using TrafficCourts.Staff.Service.Models;
using Xunit;
using TrafficCourts.Common.Errors;
using System.Security.Claims;
using System.Net;
using TrafficCourts.Coms.Client;
using TrafficCourts.Staff.Service.Models.Disputes;
using X.PagedList;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DisputeControllerTest
{
    
    [Fact]
    public async void TestGetDisputes200Result()
    {
        // Arrange
        var expected = new PagedDisputeListItemCollection(new PagedList<DisputeListItem>(Array.Empty<DisputeListItem>().AsQueryable(), 1, 25));
        var disputeService = new Mock<IDisputeService>();
        
        disputeService
            .Setup(_ => _.GetAllDisputesAsync(It.IsAny<GetAllDisputesParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        GetAllDisputesParameters? parameters = null;
        IActionResult? result = await disputeController.GetDisputesAsync(parameters!, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = Assert.IsType<PagedDisputeListItemCollection>(okResult.Value);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void TestGetDispute200Result()
    {
        // Mock the IDisputeService to return a specific Dispute for (1), confirm controller returns the dispute.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<GetDisputeOptions>(_ => _.DisputeId == id && _.Assign == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dispute, okResult.Value);
    }

    [Fact]
    public async void TestGetDispute400Result()
    {
        // Mock the IDisputeService to return a specific Dispute for (1), confirm controller returns 400 when retrieving.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<GetDisputeOptions>(_ => _.DisputeId == id && _.Assign == true), It.IsAny<CancellationToken>()))
            .Throws(new TrafficCourts.Exceptions.ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null!, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(id, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestGetDispute404Result()
    {
        // Mock the IDisputeService to return a specific Dispute for (1), confirm controller returns 404 when retrieving.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<GetDisputeOptions>(_ => _.DisputeId == id && _.Assign == true), It.IsAny<CancellationToken>()))
            .Throws(new TrafficCourts.Exceptions.ApiException("msg", StatusCodes.Status404NotFound, "rsp", null!, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(id, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async void TestUpdateDispute200Result()
    {
        // Mock the OracleDataApi to update a specific Dispute with id (1), confirm controller updates and returns the dispute.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == id), It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(id, dispute, "test comment", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dispute, okResult.Value);
    }

    [Fact]
    public async void TestUpdateDispute400Result()
    {
        // Mock the OracleDataApi to update a specific Dispute with id (1), confirm controller returns 400 when updating.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == id), It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Throws(new TrafficCourts.Exceptions.ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null!, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(id, dispute, "test comment", CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestUpdateDispute404Result()
    {
        // Mock the OracleDataApi to update a specific Dispute with id (2), confirm controller returns 404 when updating.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        long updatedId = 2;
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == updatedId), It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Throws(new TrafficCourts.Exceptions.ApiException("msg", StatusCodes.Status404NotFound, "rsp", null!, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(updatedId, dispute, "test comment", CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async void TestValidateDispute200Result()
    {
        // Mock the OracleDataApi to update a specific Dispute with id (1), confirm controller updates and returns the dispute.

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.ValidateDisputeAsync(It.Is<long>(v => v == id), null, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        await disputeController.ValidateDisputeAsync(id, null, CancellationToken.None);

        // Assert
        disputeService.VerifyAll();
    }

    [Fact]
    public async Task ValidateDisputeAsync_WithValidDispute_ShouldValidateDisputeAndUpdate()
    {
        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.ValidateDisputeAsync(It.Is<long>(v => v == id), dispute, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        await disputeController.ValidateDisputeAsync(id, dispute, CancellationToken.None);

        // Assert
        disputeService.VerifyAll();
    }

    [Fact]
    public async void TestAcceptDisputeUpdateRequest_200()
    {

        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.AcceptDisputeUpdateRequestAsync(It.Is<long>(v => v == id), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        await disputeController.AcceptDisputeUpdateRequestAsync(id, CancellationToken.None);

        // Assert
        disputeService.VerifyAll();
    }
    
    [Fact]
    public async void TestGetDisputesWithUpdateRequests200Result()
    {
        // Mock the IDisputeService to return a Disputes, confirm controller returns them.

        // Arrange
        DisputeWithUpdates dispute1 = new();
        dispute1.DisputeId = 1;
        DisputeWithUpdates dispute2 = new();
        dispute2.DisputeId = 2;
        List<DisputeWithUpdates> disputes = new() { dispute1, dispute2 };

        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetAllDisputesWithPendingUpdateRequestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(disputes);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputesWithPendingUpdateRequestsAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = okResult.Value as List<DisputeWithUpdates>;
        Assert.NotNull(actual);
        Assert.Equal(2, actual!.Count);
        Assert.Equal(dispute1, actual[0]);
        Assert.Equal(dispute2, actual[1]);
    }


    [Fact]
    public async void TestGetDisputeUpdateRequestsAsync200Result()
    {
        // Mock the IDisputeService to return a DisputeUpdateRequests, confirm controller returns them.

        // Arrange
        DisputeUpdateRequest updateRequest1 = new();
        updateRequest1.DisputeUpdateRequestId = 1;
        DisputeUpdateRequest updateRequest2 = new();
        updateRequest2.DisputeUpdateRequestId = 2;
        List<DisputeUpdateRequest> updateRequests = new() { updateRequest1, updateRequest2 };

        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeUpdateRequestsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateRequests);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeUpdateRequestsAsync(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = okResult.Value as List<DisputeUpdateRequest>;
        Assert.NotNull(actual);
        Assert.Equal(2, actual!.Count);
        Assert.Equal(updateRequest1, actual[0]);
        Assert.Equal(updateRequest2, actual[1]);
    }

    [Fact]
    public async void TestGetDisputeThrowsObjectManagementServiceException500Result()
    {
        // Arrange
        Dispute dispute = new();
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<GetDisputeOptions>(_ => _.DisputeId == id && _.Assign == true), It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(1, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error Invoking COMS"));
    }

    [Fact]
    public async void TestDeleteViolationTicketCountAsync()
    {
        // Arrange
        int violationTicketCountId = 1;
        var disputeService = new Mock<IDisputeService>();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        var mockPrintDcf = new Mock<IPrintDigitalCaseFileService>();
        DisputeController disputeController = new(disputeService.Object, mockPrintDcf.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.DeleteViolationTicketCountAsync(violationTicketCountId, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}
