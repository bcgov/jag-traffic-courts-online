using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common.Features.FilePersistence;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Features.Tickets;

public class AnalyseHandlerTests
{
    [Fact]
    public async void TestHandleReturnsResponse()
    {
        // Arrange
        var mockValidator = new Mock<IFormRecognizerValidator>();
        var mockLogger = new Mock<ILogger<AnalyseHandler.Handler>>();

        // calls AnalyzeImageAsync and Map
        AnalyzeResult expectedAnalyzeResult = FormRecognizerHelper.CreateEmptyAnalyzeResult();
        var mockService = new Mock<IFormRecognizerService>(MockBehavior.Strict);
        mockService.Setup(_ => _.AnalyzeImageAsync(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedAnalyzeResult));

        OcrViolationTicket expectedOcrViolationTicket = new();
        mockService.Setup(_ => _.Map(It.Is<AnalyzeResult>(parameter => parameter == expectedAnalyzeResult)))
            .Returns(expectedOcrViolationTicket);

        // setup the mock IFilePersistenceService
        string expectedFilename = Guid.NewGuid().ToString("n") + ".jpg";
        var filePersistenceServiceMock = new Mock<IFilePersistenceService>(MockBehavior.Strict);
        filePersistenceServiceMock
            .Setup(_ => _.SaveFileAsync(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedFilename));

        var handler = new AnalyseHandler.Handler(
            mockService.Object, 
            mockValidator.Object, 
            filePersistenceServiceMock.Object,
            new SimpleMemoryStreamManager(),
            mockLogger.Object);

        var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
        IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");

        var request = new AnalyseHandler.AnalyseRequest(file);

        // Act
        AnalyseHandler.AnalyseResponse response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedOcrViolationTicket, response.OcrViolationTicket);
        Assert.Equal(expectedFilename, response.OcrViolationTicket.ImageFilename);
    }
}
