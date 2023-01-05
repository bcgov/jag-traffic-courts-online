namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class Constructor : ObjectManagementServiceTest
{
    [Fact]
    public void should_throw_if_client_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(null!, _mockLogger.Object));
        Assert.Equal("client", actual.ParamName);
    }

    [Fact]
    public void should_throw_if_logger_is_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => new ObjectManagementService(_mockClient.Object, null!));
        Assert.Equal("logger", actual.ParamName);
    }
}
