using Microsoft.Extensions.Logging;
using Moq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

/// <summary>
/// Base class for <see cref="ObjectManagementService"/> test classes.
/// </summary>
public abstract class ObjectManagementServiceTest
{
    /// <summary>
    /// The mock client. Not readonly because some tests may want to recreate.
    /// </summary>
    protected Mock<IObjectManagementClient> _mockClient = new();
    /// <summary>
    /// The mock logger. Not readonly because some tests may want to recreate.
    /// </summary>
    protected Mock<ILogger<ObjectManagementService>> _mockLogger = new();

    /// <summary>
    /// Creates the service under test using <see cref="_mockClient"/> and <see cref="_mockLogger"/>.
    /// </summary>
    /// <returns></returns>
    protected ObjectManagementService GetService() => new(_mockClient.Object, _mockLogger.Object);

    /// <summary>
    /// Creates a <see cref="MemoryStream"/> with some random data containing the bytes of new <see cref="Guid"/>.
    /// </summary>
    /// <returns></returns>
    protected Stream GetRandomStream() => new MemoryStream(Guid.NewGuid().ToByteArray());
}
