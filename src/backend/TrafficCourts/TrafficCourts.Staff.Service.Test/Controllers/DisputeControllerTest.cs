using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DisputeControllerTest
{
    
    [Fact]
    public async void TestGetDisputes200Result()
    {
        // Mock the OracleDataApi to return a couple Disputes, confirm controller returns them.

        // Arrange
        Dispute dispute1 = new();
        dispute1.Id = 1;
        Dispute dispute2 = new();
        dispute2.Id = 1;
        List<Dispute> disputes = new List<Dispute> { dispute1, dispute2 };
        var oracleDataApi = new Mock<IOracleDataApi_v1_0Client>();
        oracleDataApi
            .Setup(_ => _.GetAllDisputesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(disputes);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(oracleDataApi.Object, mockLogger.Object);

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
        // Mock the OracleDataApi to return a specific Dispute for (1), confirm controller returns the dispute.

        // Arrange
        Dispute dispute = new();
        dispute.Id = 1;
        var oracleDataApi = new Mock<IOracleDataApi_v1_0Client>();
        oracleDataApi
            .Setup(_ => _.GetDisputeAsync(It.Is<int>(v => v == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dispute);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new (oracleDataApi.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dispute, okResult.Value);
    }

    [Fact]
    public async void TestGetDispute400Result()
    {
        // Mock the OracleDataApi to return a specific Dispute for (1), confirm controller returns 400 when retrieving.

        // Arrange
        Dispute dispute = new();
        dispute.Id = 1;
        var oracleDataApi = new Mock<IOracleDataApi_v1_0Client>();
        oracleDataApi
            .Setup(_ => _.GetDisputeAsync(It.Is<int>(v => v == 1), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(oracleDataApi.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(1, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestGetDispute404Result()
    {
        // Mock the OracleDataApi to return a specific Dispute for (1), confirm controller returns 404 when retrieving.

        // Arrange
        Dispute dispute = new();
        dispute.Id = 1;
        var oracleDataApi = new Mock<IOracleDataApi_v1_0Client>();
        oracleDataApi
            .Setup(_ => _.GetDisputeAsync(It.Is<int>(v => v == 1), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(oracleDataApi.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeAsync(1, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
