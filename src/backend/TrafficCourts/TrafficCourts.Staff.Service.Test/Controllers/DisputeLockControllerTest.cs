using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DisputeLockControllerTest
{
    [Fact]
    public async void GetLockReturns200OkWhenLockAcquiredSuccessfully()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.GetLock(It.IsAny<string>(), It.IsAny<string>())).Returns(new Lock());
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.GetLock("ticketNumber", "username");

        // Assert
        Assert.IsType<ActionResult<Lock>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
    }

    [Fact]
    public async void GetLockReturns409ConflictWhenLockIsInUse()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.GetLock(It.IsAny<string>(), It.IsAny<string>())).Throws(new LockIsInUseException("username", new Lock()));
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.GetLock("ticketNumber", "username");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        var problemDetails = Assert.IsType<LockIsInUseProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.Conflict, problemDetails.Status);
        string lockedBy = Assert.IsType<string>(problemDetails.Extensions["lockedBy"]);
        Assert.True(lockedBy == "username");
    }

    [Fact]
    public async void GetLockReturns500InternalServerErrorWhenExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.GetLock(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.GetLock("ticketNumber", "username");

        // Assert
        var errorResult = Assert.IsType<Common.Errors.HttpError>(result.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
    }

    [Fact]
    public async void RefreshLockReturns200OkWhenLockRefreshedSuccessfully()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.RefreshLock(It.IsAny<string>(), It.IsAny<string>())).Returns(DateTimeOffset.UtcNow);
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.RefreshLock("lockId");

        // Assert
        Assert.IsType<ActionResult<DateTimeOffset>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
    }

    [Fact]
    public async void RefreshLockReturns500InternalServerErrorWhenExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.RefreshLock(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.RefreshLock("lockId");

        // Assert
        var errorResult = Assert.IsType<Common.Errors.HttpError>(result.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
    }

    [Fact]
    public async void ReleaseLockReturns200OkWhenLockReleasedSuccessfully()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.ReleaseLock("lockId");

        // Assert
        Assert.IsAssignableFrom<ActionResult>(result);
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async void ReleaseLockReturns500InternalServerErrorWhenExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IDisputeLockService>();
        var mockLogger = new Mock<ILogger<DisputeLockController>>();
        mockService.Setup(x => x.ReleaseLock(It.IsAny<string>())).Throws(new Exception());
        var controller = new DisputeLockController(mockService.Object, mockLogger.Object);

        // Act
        var result = await controller.ReleaseLock("lockId");

        // Assert
        var errorResult = Assert.IsType<Common.Errors.HttpError>(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
    }

}
