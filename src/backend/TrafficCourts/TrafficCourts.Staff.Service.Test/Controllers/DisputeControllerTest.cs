using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DisputeControllerTest
{
    
    [Fact]
    public async void TestGetDisputes200Result()
    {
        // Mock the IDisputeService to return a couple Disputes, confirm controller returns them.

        // Arrange
        Guid id = Guid.NewGuid();
        Dispute dispute1 = new();
        dispute1.Id = Guid.NewGuid();
        Dispute dispute2 = new();
        dispute2.Id = Guid.NewGuid();
        Dispute dispute2 = new();
        dispute2.Id = id;
        List<Dispute> disputes = new() { dispute1, dispute2 };
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetAllDisputesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(disputes);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputesAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, ((List<Dispute>)okResult.Value).Count);
        Assert.Equal(dispute1, ((List<Dispute>)okResult.Value)[0]);
        Assert.Equal(dispute2, ((List<Dispute>)okResult.Value)[1]);
    }

    [Fact]
    public async void TestGetDispute200Result()
    {
        // Mock the IDisputeService to return a specific Dispute for (1), confirm controller returns the dispute.

        // Arrange
        Dispute dispute = new();
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<Guid>(v => v == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new (disputeService.Object, mockLogger.Object);

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
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<Guid>(v => v == id), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

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
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<Guid>(v => v == id), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

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
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<Guid>(v => v == id), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(id, dispute, CancellationToken.None);

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
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<Guid>(v => v == id), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(id, dispute, CancellationToken.None);

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
        Guid id = Guid.NewGuid();
        dispute.Id = id;
        var disputeService = new Mock<IDisputeService>();
        Guid updatedId = Guid.NewGuid();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<Guid>(v => v == updatedId), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(updatedId, dispute, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
