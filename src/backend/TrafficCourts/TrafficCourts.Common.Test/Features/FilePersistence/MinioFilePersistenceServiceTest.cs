using Microsoft.Extensions.Logging;
using Minio;
using Moq;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.FilePersistence;
using Xunit;
using Microsoft.Extensions.Options;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Minio.DataModel;
using System.Text.Json;
using AutoFixture;

namespace TrafficCourts.Common.Test.Features.FilePersistence
{
    public class MinioFilePersistenceServiceTest
    {
        private readonly Mock<ILogger<MinioFilePersistenceService>> _loggerMock = new Mock<ILogger<MinioFilePersistenceService>>();
        private readonly IMemoryStreamManager _memoryStreamManager = new SimpleMemoryStreamManager();
        private const string _bucketName = "traffic-ticket-dev";

        [Theory]
        [MemberData(nameof(FileTypes))]
        public async Task should_create_file_with_correct_filename(byte[] bytes, string extension)
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>();

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object);

            var stream = GetFile(bytes);

            var actual = await sut.SaveFileAsync(stream, CancellationToken.None);
            
            Assert.NotNull(actual);
            Assert.StartsWith(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "Pacific Standard Time").ToString("yyyy-MM-dd"), actual);
            Assert.Equal(extension, Path.GetExtension(actual));
        }

        [Fact]
        public async Task should_not_save_file_if_no_mime_type_can_be_determined()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>(MockBehavior.Strict); // do not expect any operations to be called on this

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object);

            var stream = GetFile(new byte[10]);

            var actual = await sut.SaveFileAsync(stream, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.StartsWith(String.Empty, actual);

            objectOperationsMock.VerifyAll();
        }

        [Fact]
        public async Task should_throw_if_no_data_to_save()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>();

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object);

            var stream = GetFile(new byte[0]);

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.SaveFileAsync(stream, CancellationToken.None));
            Assert.Equal("No data to save (Parameter 'data')", actual.Message);
        }

        [Fact(Skip = "Integration Test")]
        public async Task should_be_able_to_save_file_and_get_it_back()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            // be sure to run minio AND createbuckets via docker compose

            MinioClient client = new MinioClient()
                .WithEndpoint("localhost:9000")
                .WithCredentials("username", "password")
                .WithRegion("BC")
                .Build();

            var options = Options.Create(new ObjectBucketConfiguration { BucketName = _bucketName }); 

            MinioFilePersistenceService sut = new MinioFilePersistenceService(client, options, _memoryStreamManager, clock, _loggerMock.Object);
            var stream = new MemoryStream(File.ReadAllBytes("C:\\Pictures\\bear.jpg"));
            var filename = await sut.SaveFileAsync(stream, CancellationToken.None);
            var actual = await sut.GetFileAsync(filename, CancellationToken.None);

            Assert.Equal(stream.ToArray(), actual.ToArray());
        }

        [Theory]
        [MemberData(nameof(JsonObjectsWithFilename))]
        public async Task should_create_json_file_with_correct_filename<T>(T data, string filename)
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>();

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object);

            var actual = await sut.SaveJsonFileAsync(data, filename, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Equal(filename, actual);
        }

        [Fact]
        public async Task save_json_file_should_throw_if_no_data_to_save()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>();

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object);

            string filename = "";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.SaveJsonFileAsync<object>(null!, filename, CancellationToken.None));
        }

        [Theory]
        [MemberData(nameof(JsonObjectsWithNoFilename))]
        public async Task save_json_file_should_throw_if_no_filename_provided<T>(T data, string filename)
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            Mock<IObjectOperations> objectOperationsMock = new Mock<IObjectOperations>();

            IOptions<ObjectBucketConfiguration> options = Options.Create<ObjectBucketConfiguration>(new ObjectBucketConfiguration { BucketName = _bucketName });

            MinioFilePersistenceService sut = new MinioFilePersistenceService(objectOperationsMock.Object, options, _memoryStreamManager, clock, _loggerMock.Object); 

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.SaveJsonFileAsync(data, filename, CancellationToken.None));
            Assert.Equal("No filename provided to save (Parameter 'filename')", actual.Message);
        }

        [Fact(Skip = "Integration Test")]
        public async Task should_be_able_to_save_json_file_and_get_it_back()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));
            // be sure to run minio AND createbuckets via docker compose

            MinioClient client = new MinioClient()
                .WithEndpoint("localhost:9000")
                .WithCredentials("username", "password")
                .WithRegion("BC")
                .Build();

            var options = Options.Create(new ObjectBucketConfiguration { BucketName = _bucketName });

            var fixture = new Fixture();
            var ocrViolationTicket = fixture.Create<OcrViolationTicket>();
            var filename = "test-filename";

            MinioFilePersistenceService sut = new MinioFilePersistenceService(client, options, _memoryStreamManager, clock, _loggerMock.Object);
            var filenameFromStorage = await sut.SaveJsonFileAsync(ocrViolationTicket, filename, CancellationToken.None);
            var actual = await sut.GetJsonDataAsync<OcrViolationTicket>(filename, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Equal(JsonSerializer.Serialize(ocrViolationTicket), JsonSerializer.Serialize(actual));
        }

        [Fact(Skip = "Integration Test")]
        public async Task get_json_file_should_return_null_if_object_type_does_not_match()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));
            // be sure to run minio AND createbuckets via docker compose

            MinioClient client = new MinioClient()
                .WithEndpoint("localhost:9000")
                .WithCredentials("username", "password")
                .WithRegion("BC")
                .Build();

            var options = Options.Create(new ObjectBucketConfiguration { BucketName = _bucketName });

            var fixture = new Fixture();
            var violationTicket = fixture.Create<ViolationTicket>();
            var filename = "test-filename";
            Type targetType = typeof(OcrViolationTicket);
            Type actualType = typeof(ViolationTicket);

            MinioFilePersistenceService sut = new MinioFilePersistenceService(client, options, _memoryStreamManager, clock, _loggerMock.Object);
            var filenameFromStorage = await sut.SaveJsonFileAsync(violationTicket, filename, CancellationToken.None);
            var actual = await sut.GetJsonDataAsync<OcrViolationTicket>(filename, CancellationToken.None);


            Assert.Null(actual);
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Target object type to return: {targetType.FullName} does not match the type of json object fetched from object storage: {actualType.FullName}"),
                    null,
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact(Skip = "Integration Test")]
        public async Task get_json_file_should_throw_if_no_object_found()
        {
            var now = DateTimeOffset.Now;
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));
            // be sure to run minio AND createbuckets via docker compose

            MinioClient client = new MinioClient()
                .WithEndpoint("localhost:9000")
                .WithCredentials("username", "password")
                .WithRegion("BC")
                .Build();

            var options = Options.Create(new ObjectBucketConfiguration { BucketName = _bucketName });

            var fixture = new Fixture();
            var ocrViolationTicket = fixture.Create<OcrViolationTicket>();
            var filename = "test-filename";

            MinioFilePersistenceService sut = new MinioFilePersistenceService(client, options, _memoryStreamManager, clock, _loggerMock.Object);
            var filenameFromStorage = await sut.SaveJsonFileAsync(ocrViolationTicket, filename, CancellationToken.None);

            var actual = await Assert.ThrowsAsync<FileNotFoundException>(() => sut.GetJsonDataAsync<OcrViolationTicket>("incorrect-filename", CancellationToken.None));
            Assert.Equal("File not found", actual.Message);
        }

        private MemoryStream GetFile(byte[] bytes)
        {
            var stream = new MemoryStream();
            foreach (var @byte in bytes)
            {
                stream.WriteByte(@byte);
            }
            stream.Seek(0L, SeekOrigin.Begin);

            return stream;
        }

        public static IEnumerable<object[]> FileTypes
        {
            get
            {
                // https://github.com/MeaningOfLights/MimeDetect/blob/master/src/WinistaMimeDetect/mime-types.xml
                yield return new object[] { new byte[] { 0xff, 0xd8, 0xff }, ".jpg" };
                yield return new object[] { new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a }, ".png" };
            }
        }

        public static IEnumerable<object[]> JsonObjectsWithFilename()
        {
            yield return new object[] { new OcrViolationTicket(), "test-json-file" };
        }

        public static IEnumerable<object[]> JsonObjectsWithNoFilename()
        {
            yield return new object[] { new OcrViolationTicket(), "" };
            yield return new object[] { new OcrViolationTicket(), null! };
        }
    }
}
