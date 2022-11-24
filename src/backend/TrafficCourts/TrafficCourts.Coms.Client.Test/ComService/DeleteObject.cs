using AutoFixture;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace TrafficCourts.Coms.Client.Test.ComService;

public class DeleteObject : ObjectManagementBase
{
    private string OperationUrl(Guid id) => $"/api/v1/object/{id}";

    [Fact]
    public async Task should_send_request_with_correct_method_and_url()
    {
        Guid id = Guid.NewGuid();
        var expected = _fixture.Create<ResponseObjectDeleted>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Delete, OperationUrl(id))
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expected));

        ObjectManagementClient sut = GetClient(mockHttp);

        var x = await sut.DeleteObjectAsync(id, null, CancellationToken.None);
        Assert.NotNull(x);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task should_send_request_with_correct_method_and_url_and_version()
    {
        Guid id = _fixture.Create<Guid>();
        string versionId = _fixture.Create<string>();
        var expected = _fixture.Create<ResponseObjectDeleted>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Delete, OperationUrl(id))
            .WithQueryString("versionId", versionId)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expected));

        ObjectManagementClient sut = GetClient(mockHttp);

        var x = await sut.DeleteObjectAsync(id, versionId, CancellationToken.None);
        Assert.NotNull(x);
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
