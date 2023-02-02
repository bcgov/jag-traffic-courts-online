using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Moq;
using MassTransit;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services.Impl;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Services
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _redisDbAsyncMock;
        private readonly Mock<ILogger<RedisCacheService>> _loggerMock;
        private readonly IMemoryStreamManager _memoryStreamManager = new SimpleMemoryStreamManager();

        public RedisCacheServiceTests()
        {
            _redisMock = new Mock<IConnectionMultiplexer>();

            _redisDbAsyncMock = new Mock<IDatabase>();

            _loggerMock = new Mock<ILogger<RedisCacheService>>();

        }

        [Fact]
        public void Constructor_throws_ArgumentNullException_when_cache_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RedisCacheService(null!, _memoryStreamManager, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new RedisCacheService(_redisMock.Object, _memoryStreamManager, null!));
        }

        [Fact]
        public async Task GetRecordAsync_when_when_file_found()
        {
            var filename = Guid.NewGuid().ToString();
            var testData = new ViolationTicket();
            testData.TicketId = filename;
            testData.TicketNumber = "EX4343434";

            var jsonData = JsonSerializer.Serialize(testData);
            RedisValue expectedRedis = new RedisValue(jsonData);

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(expectedRedis));

            var actual = await redisCacheService.GetRecordAsync<ViolationTicket>(filename);

            Assert.NotNull(actual);

            Assert.Equal(filename, actual!.TicketId);
            Assert.Equal(testData.TicketNumber, actual.TicketNumber);
        }

        [Fact]
        public async Task GetRecordAsync_when_when_file_not_found()
        {
            var filename = Guid.NewGuid().ToString();
            RedisValue expectedRedis = new RedisValue(null!);

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(expectedRedis));

            var actual = await redisCacheService.GetRecordAsync<ViolationTicket>(filename);

            Assert.Null(actual);

        }

        [Fact]
        public async Task SetRecordAsync_sucessfully()
        {
            var key = Guid.NewGuid().ToString();
            var testData = new ViolationTicket();
            testData.TicketId = key;
            testData.TicketNumber = "EX4343434";

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);

            await redisCacheService.SetRecordAsync<ViolationTicket>(key, testData);
        }

        [Fact]
        public async Task GetFileRecordAsync_when_when_file_found()
        {
            var filename = Guid.NewGuid().ToString();
            var expected = "FileData";
            RedisValue expectedRedis = new RedisValue(expected);
            var expectedStream = System.Text.Encoding.ASCII.GetBytes(expected);

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(expectedRedis));

            var actual = await redisCacheService.GetFileRecordAsync(filename);

            Assert.NotNull(actual);
            Assert.True(actual!.CanRead);
            byte[] data = actual.ToArray();

            Assert.Equal(expectedStream, data);
        }

        [Fact]
        public async Task GetFileRecordAsync_when_when_file_not_found()
        {
            var filename = Guid.NewGuid().ToString();
            RedisValue expectedRedis = new RedisValue(null!);

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(expectedRedis));

            var actual = await redisCacheService.GetFileRecordAsync(filename);

            Assert.Null(actual);
        }

        [Fact]
        public async Task SetFileRecordAsync_save_successful()
        {
            var key = Guid.NewGuid().ToString();
            var filename = $"{key}-File";
            var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);

            var actualFilename = await redisCacheService.SetFileRecordAsync(key, fileStream);

            Assert.NotNull(actualFilename);
            Assert.Equal(filename, actualFilename);
        }

        [Fact]
        public async Task SetFileRecordAsync_null_key()
        {
            var key = Guid.NewGuid().ToString();
            var filename = $"{key}-File";
            var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);

            var actualFilename = await redisCacheService.SetFileRecordAsync(null!, fileStream);

            Assert.Equal(string.Empty,actualFilename);
        }

        [Fact]
        public async Task DeleteFileRecordAsync_delete_success()
        {
            var key = Guid.NewGuid().ToString();

            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));

            var result = await redisCacheService.DeleteRecordAsync(key);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteFileRecordAsync_null_key()
        {
            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbAsyncMock.Object);

            RedisCacheService redisCacheService = new RedisCacheService(_redisMock.Object, _memoryStreamManager, _loggerMock.Object);
            _redisDbAsyncMock.Setup(_ => _.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));

            var result = await redisCacheService.DeleteRecordAsync(null!);
            Assert.False(result);
        }
    }

}
