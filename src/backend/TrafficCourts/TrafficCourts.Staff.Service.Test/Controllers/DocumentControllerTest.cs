using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Coms.Client;
using TrafficCourts.Domain.Models;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class DocumentControllerTest
{
    [Fact]
    public async Task TestCreateAsync200Result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        FileMetadata metadata = new() { NoticeOfDisputeGuid = Guid.NewGuid().ToString("d"), FileId = Guid.NewGuid() };
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(metadata);
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult result = await sut.CreateAsync(mockFile.Object, 1, metadata.NoticeOfDisputeGuid, "Adjournment", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var metadataResult = Assert.IsType<FileMetadata>(okResult.Value);
        Assert.Equal(metadata.FileId, metadataResult.FileId);
    }

    [Fact]
    public async Task TestUploadDocumentThrowsMetadataInvalidKeyException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new MetadataInvalidKeyException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other",  CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Invalid Metadata Key"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsMetadataTooLongException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new MetadataTooLongException());
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Metadata Too Long"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsTagKeyEmptyExceptionException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new TagKeyEmptyException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Key Empty"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsTagKeyTooLongException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new TagKeyTooLongException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Key Too Long"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsTagValueTooLongException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new TagValueTooLongException(It.IsAny<string>(), It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Tag Value Too Long"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsTooManyTagsException400result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new TooManyTagsException(It.IsAny<int>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Too Many Tags"));
    }

    [Fact]
    public async Task TestUploadDocumentThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), "Other", CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error Invoking COMS"));
    }

    [Fact]
    public async Task TestDownloadDocument200Result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File file = new(fileStream, "testFile");
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        Domain.Models.DocumentProperties properties = new Domain.Models.DocumentProperties { TcoDisputeId = 1 };
        //file.Metadata.Add("ticket-number", "AO38375804");
        //file.Metadata.Add("virus-scan-status", "clean");
        var filename = file.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.GetAsync(guid, CancellationToken.None);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(filename, fileResult.FileDownloadName);
    }

    [Fact]
    public async Task TestDownloadDocumentMissingMetadataKeyThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File file = new(fileStream, "testFile");
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        //file.Metadata.Add("ticket-number", "AO38375804");
        var filename = file.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, true, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.GetAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error getting file from COMS"));
    }

    [Fact]
    public async Task TestDownloadDocumentInvalidScanStatusThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        Coms.Client.File file = new(fileStream, "testFile");
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        //mockFile.Metadata.Add("ticket-number", "AO38375804");
        //mockFile.Metadata.Add("virus-scan-status", "unscanned");
        var filename = file.FileName;
        comsService
            .Setup(_ => _.GetFileAsync(guid, true, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await sut.GetAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error getting file from COMS"));
    }

    [Fact]
    public async Task TestRemoveDocument200Result()
    {
        // Arrange
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.DeleteFileAsync(guid, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.DeleteAsync(guid, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestRemoveDocumentThrowsObjectManagementServiceException500result()
    {
        // Arrange
        var comsService = new Mock<IStaffDocumentService>();
        Guid guid = Guid.NewGuid();
        comsService
            .Setup(_ => _.DeleteFileAsync(guid, It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController comsController = new(comsService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await comsController.DeleteAsync(guid, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
        Assert.True(problemDetails?.Title?.Contains("Error removing file from COMS"));
    }
}
