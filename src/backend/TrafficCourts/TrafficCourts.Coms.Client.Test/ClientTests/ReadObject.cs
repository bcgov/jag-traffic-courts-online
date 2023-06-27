using AutoFixture;
using RichardSzalay.MockHttp;
using System.Net;

namespace TrafficCourts.Coms.Client.Test.ClientTests;

public class ReadObject : ObjectManagementBase
{
    [Fact]
    public async Task should_send_request_with_correct_method_and_url()
    {
        Guid id = _fixture.Create<Guid>();
        var download = _fixture.Create<DownloadMode>();
        var expiresIn = Random.Shared.Next(1, short.MaxValue);
        var versionId = _fixture.Create<string>();

        // DownloadMode? download, int? expiresIn, string? versionId
        // can be 200 or 206
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Get, $"/api/v1/object/{id}")
            .WithQueryString(new Dictionary<string, string>
            {
                { "download", download.ToString().ToLower() }, // not totally correct, but values are lower case
                { "expiresIn", expiresIn.ToString() },
                { "s3VersionId", versionId },
            })
            .Respond(HttpStatusCode.OK, "application/octet-stream", new MemoryStream(Guid.Empty.ToByteArray()));

        HttpClient client = mockHttp.ToHttpClient();

        ObjectManagementClient sut = new ObjectManagementClient(client);
        sut.BaseUrl = "http://localhost/api/v1";

        var file = await sut.ReadObjectAsync(id, download, expiresIn, versionId, CancellationToken.None);
        Assert.NotNull(file);
        mockHttp.VerifyNoOutstandingExpectation();
    }
}

public class UpdateObject
{

}