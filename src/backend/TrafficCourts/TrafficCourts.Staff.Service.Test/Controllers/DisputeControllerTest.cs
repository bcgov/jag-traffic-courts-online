using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DisputeControllerTest
{

    [Fact]
    public async void TestGetDisputeOkResult()
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
}
