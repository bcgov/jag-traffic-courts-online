using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.FilePersistence;
using Xunit;

namespace TrafficCourts.Common.Test.Features.FilePersistence;

public class InMemoryFilePersistenceServiceTest
{
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<InMemoryFilePersistenceService>> _loggerMock;

    public InMemoryFilePersistenceServiceTest()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _cache = serviceProvider.GetRequiredService<IMemoryCache>();
        _loggerMock = new Mock<ILogger<InMemoryFilePersistenceService>>();
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_cache_null()
    {
        Assert.Throws<ArgumentNullException>(() => new InMemoryFilePersistenceService(null!, _loggerMock.Object));
        Assert.Throws<ArgumentNullException>(() => new InMemoryFilePersistenceService(_cache, null!));
    }

    [Fact]
    public async Task GetFileAsync_when_when_file_found()
    {
        var filename = Guid.NewGuid().ToString();
        var expected = Guid.NewGuid().ToByteArray();

        _cache.Set(filename, expected);

        InMemoryFilePersistenceService sut = new(_cache, _loggerMock.Object);

        var actual = await sut.GetFileAsync(filename, CancellationToken.None);
        Assert.NotNull(actual);
        Assert.True(actual.CanRead);
        byte[] data = actual.ToArray();

        Assert.Equal(expected, data);
    }

    [Fact]
    public async Task GetFileAsync_throws_FileNotFoundException_when_file_not_found()
    {
        var filename = Guid.NewGuid().ToString();

        InMemoryFilePersistenceService sut = new(_cache, _loggerMock.Object);

        await Assert.ThrowsAsync<FileNotFoundException>(() => sut.GetFileAsync(filename, CancellationToken.None));
    }
}
