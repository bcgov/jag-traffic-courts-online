using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Services
{
    public class ArcFileServiceTests
    {
        private Mock<ISftpService> _mockArcFileService;
        private readonly ILogger<ArcFileService> _logger;
        private IMemoryStreamManager _memoryStreamManager = new SimpleMemoryStreamManager();

        public ArcFileServiceTests()
        {
            _mockArcFileService = new Mock<ISftpService>();
            _logger = new Mock<ILogger<ArcFileService>>().Object;
        }

        [Fact]
        public async void TestCreateStreamFromArcData()
        {
            // Arrange
            SftpOptions options = new SftpOptions();
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            var records = new List<ArcFileRecord>();
            records.Add(new AdnotatedTicket());

            // Act
            var actual = await arcFileService.CreateStreamFromArcData(records, CancellationToken.None);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Position);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public async void Test_throw_ArgumentNullException_if_null_value_passed_for_CreateArcFile()
        {
            // Arrange
            SftpOptions options = new SftpOptions();
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            await Assert.ThrowsAsync<ArgumentNullException>(() => arcFileService.CreateArcFile(null!, CancellationToken.None));
        }


        [Fact]
        public async void when_creating_arc_file_it_is_upload_to_correct_remote_path()
        {
            // Arrange
            SftpOptions options = new SftpOptions { RemotePath = Guid.NewGuid().ToString("n") };
            string fileNumber = Guid.NewGuid().ToString("n");

            _mockArcFileService = new Mock<ISftpService>(MockBehavior.Strict);

            // expecte the uploaded file to be uploaded to the remote path, using the file number with extension .txt
            string expectedFilename = "dispute-" + fileNumber + ".txt";
            _mockArcFileService.Setup(service => service.UploadFile(
                It.IsAny<MemoryStream>(),
                It.Is<string>(_ => _ == options.RemotePath),
                It.Is<string>(_ => _ == expectedFilename)
            ));

            var records = new List<ArcFileRecord> { new AdnotatedTicket { FileNumber = fileNumber } };
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            // Act
            await arcFileService.CreateArcFile(records, CancellationToken.None);
        }
    }
}
