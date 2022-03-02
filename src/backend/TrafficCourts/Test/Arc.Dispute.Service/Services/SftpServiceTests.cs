using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Services
{
    // Due to the lack of interface and abstraction support of Renci.SshNet library, this unit test coverage has been removed from the context.
    // If there will be interface support added for the connect methods of the library in the future or a wrapper will be implemented, the tests can be enabled. 
    /*
    public class SftpServiceTests
    {
        private readonly Mock<ILogger<SftpService>> _mockLogger;
        private readonly Mock<SftpClient> _mockClient;
        private readonly Fixture _fixture;

        public SftpServiceTests()
        {
            _mockLogger = new Mock<ILogger<SftpService>>();
            _mockClient = new Mock<SftpClient>();
            _fixture = new Fixture();
        }

        [Fact]
        public void TestUploadFileSuccess()
        {
            // Arrange
            var mockMemoryStream = new Mock<MemoryStream>();
            string fileName = _fixture.Create<String>();
            string filePath = @"./" + fileName;
            var arcSftpService = new SftpService(_mockLogger.Object, _mockClient.Object);



            // Act
            arcSftpService.UploadFile(mockMemoryStream.Object, filePath);

            // Assert
            _mockLogger.Verify(_ => _.LogDebug("Finished uploading file to [{FilePath}]", filePath), Times.Once);
        }

        [Fact]
        public async void TestThrowArgumentNullExceptionIfNullValuesPassedForUploadFile()
        {
            var mockMemoryStream = new Mock<MemoryStream>();
            string fileName = _fixture.Create<String>();
            string filePath = @"./" + fileName;
            var arcSftpService = new SftpService(_mockLogger.Object, _mockClient.Object);

            Assert.Throws<ArgumentNullException>(() => arcSftpService.UploadFile(null, filePath));
            Assert.Throws<ArgumentNullException>(() => arcSftpService.UploadFile(mockMemoryStream.Object, null));
        }
    }
    */
}
