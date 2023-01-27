using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Workflow.Service.Services;
using Moq;
using Xunit;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Messaging.MessageContracts;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TrafficCourts.Test.Workflow.Service.Services
{
    public class VirusScanServiceTests
    {
        private readonly Mock<ILogger<VirusScanService>> _mockLogger;
        private readonly Mock<IVirusScanClient> _mockVirusScanClient;

        public VirusScanServiceTests()
        {
            _mockLogger = new Mock<ILogger<VirusScanService>>();
            _mockVirusScanClient = new Mock<IVirusScanClient>();
        }

        /// <summary>
        /// Creates VirusScanService
        /// </summary>
        /// <returns></returns>
        private VirusScanService CreateService()
        {
            return new VirusScanService(
                _mockLogger.Object,
                _mockVirusScanClient.Object);
        }

        [Fact]
        public async Task ScanDocumentAsync_WithCorrectParams_ShouldReturnScanResponse()
        {
            // Arrange
            var virusScanMessage = new VirusScanDocument
            {
                DocumentId = Guid.NewGuid()
            };

            ScanResponse resp = new ScanResponse
            {
                Status = VirusScanStatus.NotInfected
            };

            _mockVirusScanClient.Setup(client => client.ScanFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(resp));

            var sut = CreateService();

            // Act
            var result = await sut.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>());

            // Assert
            var scanResult = Assert.IsType<ScanResponse>(result);
            Assert.Equal(VirusScanStatus.NotInfected, scanResult.Status);
        }

        [Fact]
        public async Task ScanDocumentAsync_WithInternalServerError_ShouldThrowApiException()
        {
            // Arrange
            var virusScanMessage = new VirusScanDocument
            {
                DocumentId = Guid.NewGuid()
            };

            ScanResponse resp = new ScanResponse
            {
                Status = VirusScanStatus.NotInfected
            };

            _mockVirusScanClient.Setup(client => client.ScanFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new ApiException("There was an internal error virus scanning the file.", StatusCodes.Status500InternalServerError, It.IsAny<string>(), null, null));

            var sut = CreateService();

            // Act
            var result = sut.ScanDocumentAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>());

            try
            {
                await result;
            }
            catch (ApiException ex)
            {
                // Assert
                Assert.Equal(StatusCodes.Status500InternalServerError, ex.StatusCode);
                Assert.Contains("There was an internal error virus scanning the file.", ex.Message);
            }

            // Assert
            Assert.False(result.IsCompletedSuccessfully);
        }

    }
}
