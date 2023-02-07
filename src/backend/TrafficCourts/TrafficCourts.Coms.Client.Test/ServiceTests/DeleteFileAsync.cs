#nullable disable
using Moq;
using System;

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

    [Fact]
    public async Task should_not_throw_error_if_client_returns_not_found_error()
    {
        Guid id = Guid.NewGuid();
        CancellationTokenSource cts = new CancellationTokenSource();

        ResponseError error = new ResponseError { Detail = "NotFoundError" };
        ApiException<ResponseError> exception = new ApiException<ResponseError>("", 502, null, null, error, null);

        _mockClient.Setup(_ => _.DeleteObjectAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => exception);

        ObjectManagementService sut = GetService();

        await sut.DeleteFileAsync(id, cts.Token);
    }

    [Fact]
    public async Task should_ObjectManagementServiceException_when_other_erorr_thrown()
    {
        Guid id = Guid.NewGuid();
        CancellationTokenSource cts = new CancellationTokenSource();

        ResponseError error = new ResponseError { Detail = "OtherError" };
        ApiException<ResponseError> exception = new ApiException<ResponseError>("", 502, null, null, error, null);

        _mockClient.Setup(_ => _.DeleteObjectAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => exception);

        ObjectManagementService sut = GetService();

        var actual = await Assert.ThrowsAsync<ObjectManagementServiceException>(() => sut.DeleteFileAsync(id, cts.Token));
    }
}
