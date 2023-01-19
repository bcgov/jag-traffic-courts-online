using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using TrafficCourts.Coms.Client;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class ComsControllerTest
{
    [Fact]
    public async void TestUploadDocument200Result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(guid);
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(guid, okResult.Value);
    }

    [Fact]
    public async void TestUploadDocumentMissingTicketnumberMetadataThrows400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Metadata Key does not contain ticketnumber"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsMetadataInvalidKeyException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new MetadataInvalidKeyException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Invalid Metadata Key"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsMetadataTooLongException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new MetadataTooLongException());
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Metadata Too Long"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsTagKeyEmptyExceptionException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new TagKeyEmptyException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Key Empty"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsTagKeyTooLongException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new TagKeyTooLongException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Key Too Long"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsTagValueTooLongException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new TagValueTooLongException(It.IsAny<string>(), It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Value Too Long"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsTooManyTagsException400result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new TooManyTagsException(It.IsAny<int>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Too Many Tags"));
    }

    [Fact]
    public async void TestUploadDocumentThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var mockFileUploadRequest = new Mock<FileUploadRequest>();
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFileUploadRequest.Object.Metadata.Add("ticketnumber", "AO38375804");
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.UploadDocumentAsync(mockFileUploadRequest.Object, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error Invoking COMS"));
    }

    [Fact]
    public async void TestDownloadDocument200Result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File mockFile = new(fileStream, "testFile");
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFile.Metadata.Add("ticketnumber", "AO38375804");
        mockFile.Metadata.Add("virus-scan-status", "clean");
        var filename = mockFile.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockFile);
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.DownloadDocumentAsync(guid, CancellationToken.None);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(filename, fileResult.FileDownloadName);
    }

    [Fact]
    public async void TestDownloadDocumentMissingMetadataKeyThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File mockFile = new(fileStream, "testFile");
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFile.Metadata.Add("ticketnumber", "AO38375804");
        var filename = mockFile.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.DownloadDocumentAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error getting file from COMS"));
    }

    [Fact]
    public async void TestDownloadDocumentInvalidScanStatusThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File mockFile = new(fileStream, "testFile");
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        mockFile.Metadata.Add("ticketnumber", "AO38375804");
        mockFile.Metadata.Add("virus-scan-status", "unscanned");
        var filename = mockFile.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.DownloadDocumentAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error getting file from COMS"));
    }

    [Fact]
    public async void TestRemoveDocument200Result()
    {
        // Arrange
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.DeleteFileAsync(guid, It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.RemoveDocumentAsync(guid, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestRemoveDocumentThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var comsService = new Mock<IComsService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.DeleteFileAsync(guid, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<ComsController>>();
        ComsController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.RemoveDocumentAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error removing file from COMS"));
    }
}
