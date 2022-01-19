using Azure.AI.FormRecognizer.DocumentAnalysis;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using TrafficCourts.Citizen.Service.Services;
using Xunit;

namespace TrafficCourts.Citizen.Service.Features.Tickets;

public class AnalyseHandlerTests
{

    [Fact]
    public async void TestHandleReturnsResponse()
    {
        // Arrange
        var mockService = new Mock<IFormRecognizerService>();
        var mockLogger = new Mock<ILogger<AnalyseHandler.Handler>>();
        var handler = new AnalyseHandler.Handler(mockService.Object, mockLogger.Object);
        
        var mockImage = new Mock<IFormFile>();
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);

        // Act
        AnalyseHandler.AnalyseResponse response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }

}