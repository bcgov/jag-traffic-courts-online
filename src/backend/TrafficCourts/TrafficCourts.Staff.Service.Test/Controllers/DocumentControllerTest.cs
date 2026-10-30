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
using TrafficCourts.Domain.Enums;
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
        IActionResult result = await sut.CreateAsync(mockFile.Object, 1, metadata.NoticeOfDisputeGuid, UploadedDocumentType.Adjournment.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(),  CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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
        IActionResult? result = await sut.CreateAsync(mockFile.Object, 1, guid.ToString("d"), UploadedDocumentType.Other.GetDescription(), CancellationToken.None);

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

    [Theory]
    [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Pending)]
    [InlineData(UploadedDocumentType.Other, DocumentStatus.Filed)]
    [InlineData(UploadedDocumentType.WrittenReasons, DocumentStatus.Filed)]
    [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Filed)]
    public async Task TestCreateAsyncInfersDocumentStatusFromDocumentType(UploadedDocumentType documentType, DocumentStatus expectedStatus)
    {
        DocumentProperties? saved = await CreateAndCaptureProperties(documentType.GetDescription());

        Assert.NotNull(saved);
        Assert.Equal(documentType.GetDescription(), saved.DocumentType);
        Assert.Equal(expectedStatus, saved.DocumentStatus);
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("adjournment")]
    public async Task TestCreateAsyncUnrecognizedDocumentTypeIsFiled(string documentType)
    {
        DocumentProperties? saved = await CreateAndCaptureProperties(documentType);

        Assert.NotNull(saved);
        Assert.Equal(documentType, saved.DocumentType);
        Assert.Equal(DocumentStatus.Filed, saved.DocumentStatus);
    }

    [Theory]
    [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Pending)]
    [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Resolved)]
    [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Filed)]
    [InlineData(UploadedDocumentType.Other, DocumentStatus.Filed)]
    [InlineData(UploadedDocumentType.WrittenReasons, DocumentStatus.Filed)]
    [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Filed)]
    public async Task TestUpdateAsyncValidTypeAndStatus200Result(UploadedDocumentType documentType, DocumentStatus documentStatus)
    {
        // Arrange
        var comsService = new Mock<IStaffDocumentService>();
        Guid fileId = Guid.NewGuid();
        DocumentController sut = CreateControllerWithHttpContext(comsService.Object);

        // Act
        IActionResult result = await sut.UpdateAsync(fileId, documentType.GetDescription(), documentStatus, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
        comsService.Verify(_ => _.UpdateFileAsync(
            fileId,
            It.Is<DocumentProperties>(p => p.DocumentType == documentType.GetDescription() && p.DocumentStatus == documentStatus),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(UploadedDocumentType.Other, DocumentStatus.Pending)]
    [InlineData(UploadedDocumentType.Other, DocumentStatus.Resolved)]
    [InlineData(UploadedDocumentType.WrittenReasons, DocumentStatus.Pending)]
    [InlineData(UploadedDocumentType.WrittenReasons, DocumentStatus.Resolved)]
    [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Pending)]
    [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Resolved)]
    public async Task TestUpdateAsyncInvalidStatusForType400Result(UploadedDocumentType documentType, DocumentStatus documentStatus)
    {
        // Arrange
        var comsService = new Mock<IStaffDocumentService>();
        DocumentController sut = CreateControllerWithHttpContext(comsService.Object);

        // Act
        IActionResult result = await sut.UpdateAsync(Guid.NewGuid(), documentType.GetDescription(), documentStatus, CancellationToken.None);

        // Assert
        AssertInvalidParameter(result, "documentStatus");
        comsService.Verify(_ => _.UpdateFileAsync(It.IsAny<Guid>(), It.IsAny<DocumentProperties>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("adjournment")]
    [InlineData(" Adjournment")]
    [InlineData("WrittenReasons")]
    [InlineData("CertifiedExtract")]
    public async Task TestUpdateAsyncUnknownDocumentType400Result(string documentType)
    {
        // Arrange
        var comsService = new Mock<IStaffDocumentService>();
        DocumentController sut = CreateControllerWithHttpContext(comsService.Object);

        // Act
        IActionResult result = await sut.UpdateAsync(Guid.NewGuid(), documentType, DocumentStatus.Filed, CancellationToken.None);

        // Assert
        AssertInvalidParameter(result, "documentType");
        comsService.Verify(_ => _.UpdateFileAsync(It.IsAny<Guid>(), It.IsAny<DocumentProperties>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static async Task<DocumentProperties?> CreateAndCaptureProperties(string documentType)
    {
        var mockFile = new Mock<IFormFile>();
        var comsService = new Mock<IStaffDocumentService>();
        DocumentProperties? saved = null;
        comsService
            .Setup(_ => _.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<DocumentProperties>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()))
            .Callback<IFormFile, DocumentProperties, ClaimsPrincipal, CancellationToken>((_, properties, _, _) => saved = properties)
            .ReturnsAsync(new FileMetadata());
        var mockLogger = new Mock<ILogger<DocumentController>>();
        DocumentController sut = new(comsService.Object, mockLogger.Object);

        IActionResult result = await sut.CreateAsync(mockFile.Object, 1, Guid.NewGuid().ToString("d"), documentType, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
        return saved;
    }

    /// <summary>
    /// UpdateAsync error handlers dereference HttpContext.Request.Path, so an HttpContext is required.
    /// </summary>
    private static DocumentController CreateControllerWithHttpContext(IStaffDocumentService documentService)
    {
        var mockLogger = new Mock<ILogger<DocumentController>>();
        return new DocumentController(documentService, mockLogger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }

    private static void AssertInvalidParameter(IActionResult result, string parameterName)
    {
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
        Assert.Equal("Invalid Parameter: " + parameterName, problemDetails.Detail);
    }
}
