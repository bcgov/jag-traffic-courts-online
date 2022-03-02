using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Services
{
    public class ArcFileServiceTests
    {
        private readonly Mock<ISftpService> _mockArcFileService;

        public ArcFileServiceTests()
        {
            _mockArcFileService = new Mock<ISftpService>();
        }

        [Fact]
        public async void TestCreateStreamFromArcData()
        {
            // Arrange
            var records = new List<ArcFileRecord>();
            records.Add(new AdnotatedTicket());
            var arcFileService = new ArcFileService(_mockArcFileService.Object);

            // Act
            var actual = await arcFileService.CreateStreamFromArcData(records);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Position);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public async void Test_throw_ArgumentNullException_if_null_value_passed_for_CreateArcFile()
        {
            var arcFileService = new ArcFileService(_mockArcFileService.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => arcFileService.CreateArcFile(null));
        }
    }
}
