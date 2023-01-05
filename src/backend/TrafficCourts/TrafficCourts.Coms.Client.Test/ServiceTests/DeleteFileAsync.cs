using Moq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class DeleteFileAsync : ObjectManagementServiceTest
{
    [Fact]
    public async Task should_pass_parameters_to_client()
    {
        Guid id = Guid.NewGuid();
        CancellationTokenSource cts = new CancellationTokenSource();

        ObjectManagementService sut = GetService();

        await sut.DeleteFileAsync(id, cts.Token);

        // verify the client called with correct values
        _mockClient.Verify(_ =>
            _.DeleteObjectAsync(
                It.Is<Guid>((actual) => actual == id),
                It.Is<string>((actual) => actual == null),
                It.Is<CancellationToken>((actual) => actual == cts.Token)
            ));
    }
}
