using FluentAssertions;
using FluentAssertions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Controllers;
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
        Dispute dispute1 = new();
        dispute1.DisputeId = 1;
        Dispute dispute2 = new();
        dispute2.DisputeId =2;
        List<Dispute> disputes = new() { dispute1, dispute2 };
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetAllDisputesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(disputes);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputesAsync(null, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = okResult.Value as List<Dispute>;
        Assert.NotNull(actual);
        Assert.Equal(2, actual!.Count);
        Assert.Equal(dispute1, actual[0]);
        Assert.Equal(dispute2, actual[1]);
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
            .Setup(_ => _.GetDisputeAsync(It.Is<long>(v => v == id), It.IsAny<CancellationToken>()))
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
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<long>(v => v == id), It.IsAny<CancellationToken>()))
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
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeAsync(It.Is<long>(v => v == id), It.IsAny<CancellationToken>()))
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
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == id), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
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
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == id), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
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
        long id = 1;
        dispute.DisputeId = id;
        var disputeService = new Mock<IDisputeService>();
        long updatedId = 2;
        disputeService
            .Setup(_ => _.UpdateDisputeAsync(It.Is<long>(v => v == updatedId), It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.UpdateDisputeAsync(updatedId, dispute, CancellationToken.None);

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
            .Setup(_ => _.ValidateDisputeAsync(It.Is<long>(v => v == id), It.IsAny<CancellationToken>()))
            .Verifiable();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        await disputeController.ValidateDisputeAsync(id, CancellationToken.None);

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
            .Setup(_ => _.AcceptDisputeUpdateRequestAsync(It.Is<long>(v => v == id), It.IsAny<CancellationToken>()))
            .Verifiable();
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

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
        Dispute dispute1 = new();
        dispute1.DisputeId = 1;
        Dispute dispute2 = new();
        dispute2.DisputeId = 2;
        List<Dispute> disputes = new() { dispute1, dispute2 };

        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetAllDisputesWithPendingUpdateRequestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(disputes);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputesWithPendingUpdateRequestsAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = okResult.Value as List<Dispute>;
        Assert.NotNull(actual);
        Assert.Equal(2, actual!.Count);
        Assert.Equal(dispute1, actual[0]);
        Assert.Equal(dispute2, actual[1]);
    }


    [Fact]
    public async void TestGetDisputeUpdateRequestsAsync200Result()
    {
        // Mock the IDisputeService to return a DisputantUpdateRequests, confirm controller returns them.

        // Arrange
        DisputantUpdateRequest updateRequest1 = new();
        updateRequest1.DisputantUpdateRequestId = 1;
        DisputantUpdateRequest updateRequest2 = new();
        updateRequest2.DisputantUpdateRequestId = 2;
        List<DisputantUpdateRequest> updateRequests = new() { updateRequest1, updateRequest2 };

        var disputeService = new Mock<IDisputeService>();
        disputeService
            .Setup(_ => _.GetDisputeUpdateRequestsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateRequests);
        var mockLogger = new Mock<ILogger<DisputeController>>();
        DisputeController disputeController = new(disputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await disputeController.GetDisputeUpdateRequestsAsync(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var actual = okResult.Value as List<DisputantUpdateRequest>;
        Assert.NotNull(actual);
        Assert.Equal(2, actual!.Count);
        Assert.Equal(updateRequest1, actual[0]);
        Assert.Equal(updateRequest2, actual[1]);
    }
    [Fact]
    public void AllEndpointsShouldImplementAuthorizeAttribute()
    {
        // Check all endpoints of DisputeController to confirm all are guarded with proper KeycloakAuthorization or explicit AllowAnonymous Attribute

        // Arrange
        var _endpoints = new List<(Type, MethodInfo)>(); // All endpoints to check in DisputeController

        var assembly = Assembly.GetAssembly(typeof(DisputeController));
        var allControllers = AllTypes.From(assembly).ThatDeriveFrom<StaffControllerBase<DisputeController>>();

        foreach (Type type in allControllers)
        {
            var mInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.DeclaringType!.Equals(type));
            foreach (MethodInfo mInfo in mInfos)
            {
                _endpoints.Add((type, mInfo));
            }
        }

        // Act
        var endpointsWithoutAuthorizeAttribute = _endpoints.Where(t => !t.Item2.IsDefined(typeof(KeycloakAuthorizeAttribute), false) && !t.Item2.IsDefined(typeof(AllowAnonymousAttribute), false)).ToList();
        var brokenEndpoints = string.Join(" and ", endpointsWithoutAuthorizeAttribute.Select(x => x.Item2.Name));

        // Assert
        endpointsWithoutAuthorizeAttribute.Count.Should().Be(0, "because {0} should have the KeycloakAuthorization or Anonymous attribute", brokenEndpoints);
    }
}
