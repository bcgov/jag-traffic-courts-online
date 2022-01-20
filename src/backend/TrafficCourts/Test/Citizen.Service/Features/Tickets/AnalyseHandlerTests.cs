using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using System.Threading;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Features.Tickets;

public class AnalyseHandlerTests
{

    [Fact]
    public async void TestHandleReturnsResponse()
    {
        // Arrange
        var mockService = new Mock<IFormRecognizerService>();
        var mockValidator = new Mock<IFormRecognizerValidator>();
        var mockLogger = new Mock<ILogger<AnalyseHandler.Handler>>();
        var handler = new AnalyseHandler.Handler(mockService.Object, mockValidator.Object, mockLogger.Object);
        
        var mockImage = new Mock<IFormFile>();
        var request = new AnalyseHandler.AnalyseRequest(mockImage.Object);

        // Act
        AnalyseHandler.AnalyseResponse response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }

}