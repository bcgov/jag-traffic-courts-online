using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Coms.Client.Test.ObjectManagementServiceTests;

public abstract class ObjectManagementServiceTest
{
    protected Mock<IObjectManagementClient> _mockClient = new();
    protected Mock<ILogger<ObjectManagementService>> _mockLogger = new Mock<ILogger<ObjectManagementService>>();

    protected ObjectManagementService GetService() => new(_mockClient.Object, _mockLogger.Object);

    protected Stream GetRandomStream() => new MemoryStream(Guid.NewGuid().ToByteArray());
}
