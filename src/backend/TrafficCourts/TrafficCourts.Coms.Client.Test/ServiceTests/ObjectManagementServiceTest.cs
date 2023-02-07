using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Coms.Client.Data;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

/// <summary>
/// Base class for <see cref="ObjectManagementService"/> test classes.
/// </summary>
public abstract class ObjectManagementServiceTest
{
    protected readonly Fixture _fixture = new Fixture();

    /// <summary>
    /// The mock client. Not readonly because some tests may want to recreate.
    /// </summary>
    protected Mock<IObjectManagementClient> _mockClient = new();

    protected Mock<IObjectManagementRepository> _mockRepository = new();

    /// <summary>
    /// Factory to create memory streams
    /// </summary>
    protected readonly IMemoryStreamFactory _memoryStreamFactory = new MemoryStreamFactory(() => new MemoryStream());

    /// <summary>
    /// The mock logger. Not readonly because some tests may want to recreate.
    /// </summary>
    internal Mock<ILogger<ObjectManagementService>> _mockLogger = new();

    /// <summary>
    /// Creates the service under test using <see cref="_mockClient"/> and <see cref="_mockLogger"/>.
    /// </summary>
    /// <returns></returns>
    internal ObjectManagementService GetService() => new(_mockClient.Object, _mockRepository.Object, _memoryStreamFactory, _mockLogger.Object);

    /// <summary>
    /// Creates a <see cref="MemoryStream"/> with some random data containing the bytes of new <see cref="Guid"/>.
    /// </summary>
    /// <returns></returns>
    protected Stream GetRandomStream() => new MemoryStream(Guid.NewGuid().ToByteArray());

    protected void SetupReadObjectReturn(FileResponse response)
    {
        // Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken)
        _mockClient.Setup(_ => _.ReadObjectAsync(
                It.IsAny<Guid>(),
                It.IsAny<DownloadMode?>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => response);
    }

    protected void SetupGetObjectMetadataAsync(IList<ObjectMetadata> response)
    {
        _mockClient.Setup(_ => _.GetObjectMetadataAsync(
                It.IsAny<IList<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => response);
    }


    protected static bool Equal(IReadOnlyDictionary<string, string> expected, IReadOnlyDictionary<string, string> actual)
    {
        if (expected.Count != actual.Count)
        {
            return false;
        }

        foreach (var item in expected)
        {
            if (!actual.ContainsKey(item.Key) || actual[item.Key] != item.Value)
            {
                return false;
            }
        }

        return true;
    }
}
