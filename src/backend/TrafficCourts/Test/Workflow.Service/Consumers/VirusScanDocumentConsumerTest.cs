using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using Xunit;
using ApiException = TrafficCourts.Common.OpenAPIs.VirusScan.V1.ApiException;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class VirusScanDocumentConsumerTest
{
    private readonly VirusScanDocument _message;
    private readonly Coms.Client.File _file;
    private readonly Mock<ILogger<VirusScanDocumentConsumer>> _mockLogger;
    private readonly Mock<IVirusScanService> _virusScanService;
    private readonly Mock<IComsService> _comsService;
    private readonly Mock<ConsumeContext<VirusScanDocument>> _context;
    private readonly VirusScanDocumentConsumer _consumer;
    private readonly Guid _guid;

    public VirusScanDocumentConsumerTest()
    {
        _guid= Guid.NewGuid();
        var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
        _message = new()
        {
            DocumentId = _guid
        };
        _file = new(_guid, fileStream, "sample_file", null, null, null);

        _mockLogger = new();
        _virusScanService = new();
        _comsService = new();
        _context = new();
        _context.Setup(_ => _.Message).Returns(_message);
        _context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);

        _consumer = new(_virusScanService.Object, _comsService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultClean()
    {
        // Arrange
        _comsService.Setup(_ => _.GetFileAsync(_guid, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_file));

        ScanResponse response = new()
        {
            Status = VirusScanStatus.NotInfected
        };

        _virusScanService.Setup(_ => _.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        string result = _file.Metadata.GetValueOrDefault("virus-scan-status") ?? "";
        Assert.Equal("clean", result);
    }

    [Fact]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultInfected()
    {
        // Arrange
        _comsService.Setup(_ => _.GetFileAsync(_guid, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_file));

        ScanResponse response = new()
        {
            Status = VirusScanStatus.Infected,
            VirusName = "cryptolocker"
        };

        _virusScanService.Setup(_ => _.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        string result = _file.Metadata.GetValueOrDefault("virus-scan-status") ?? "";
        Assert.Equal("infected", result);
        string virusName = _file.Metadata.GetValueOrDefault("virus-name") ?? "";
        Assert.Equal("cryptolocker", virusName);
    }

    [Fact]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultUnknown()
    {
        // Arrange
        _comsService.Setup(_ => _.GetFileAsync(_guid, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_file));

        ScanResponse response = new()
        {
            Status = VirusScanStatus.Unknown
        };

        _virusScanService.Setup(_ => _.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        string result = _file.Metadata.GetValueOrDefault("virus-scan-status") ?? "";
        Assert.Equal("Unknown", result);
    }

    [Fact]
    public async Task TestVirusScanDocumentConsumer_ThrowsObjectManagementServiceException()
    {
        // Arrange
        _comsService.Setup(_ => _.GetFileAsync(_guid, It.IsAny<CancellationToken>()))
            .Throws(new ObjectManagementServiceException(It.IsAny<string>()));

        // Act
        var result = _consumer.Consume(_context.Object);

        try
        {
            await result;
        }
        catch (ObjectManagementServiceException e)
        {
            // Assert
            Assert.IsType<ObjectManagementServiceException>(e);
        }

        // Assert
        Assert.False(result.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task TestVirusScanDocumentConsumer_ThrowsApiException()
    {
        // Arrange
        _comsService.Setup(_ => _.GetFileAsync(_guid, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_file));

        _virusScanService.Setup(_ => _.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException("There was an internal error virus scanning the file.", StatusCodes.Status500InternalServerError, It.IsAny<string>(), null, null));

        // Act
        var result = _consumer.Consume(_context.Object);

        try
        {
            await result;
        }
        catch (ApiException e)
        {
            // Assert
            Assert.IsType<ApiException>(e);
            Assert.Equal(StatusCodes.Status500InternalServerError, e.StatusCode);
            Assert.Contains("There was an internal error virus scanning the file.", e.Message);
        }

        // Assert
        Assert.False(result.IsCompletedSuccessfully);
    }
}
