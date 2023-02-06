namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class Constructor : ObjectManagementServiceTest
{
    [Fact]
    public void should_throw_if_client_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(null!, _mockRepository.Object, _memoryStreamFactory, _mockLogger.Object));
        Assert.Equal("client", actual.ParamName);
    }

    [Fact]
    public void should_throw_if_repository_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(_mockClient.Object, null!,_memoryStreamFactory, _mockLogger.Object));
        Assert.Equal("repository", actual.ParamName);
    }

    [Fact]
    public void should_throw_if_memoryStreamFactory_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(_mockClient.Object, _mockRepository.Object, null!, _mockLogger.Object));
        Assert.Equal("memoryStreamFactory", actual.ParamName);
    }

    [Fact]
    public void should_throw_if_logger_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(_mockClient.Object, _mockRepository.Object, _memoryStreamFactory, null!));
        Assert.Equal("logger", actual.ParamName);
    }
}
