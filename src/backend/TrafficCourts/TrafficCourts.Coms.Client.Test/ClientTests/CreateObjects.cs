using AutoFixture;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace TrafficCourts.Coms.Client.Test.ClientTests;

public class CreateObjects : ObjectManagementBase
{
    private const string OperationUrl = "/api/v1/object";

    [Fact]
    public async Task should_require_file_parameter()
    {
        var mockHttp = new MockHttpMessageHandler();

        ObjectManagementClient sut = new ObjectManagementClient(mockHttp.ToHttpClient());

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CreateObjectsAsync(null, null, null!, CancellationToken.None));
        Assert.Equal("anyKey", actual.ParamName);
    }

    [Fact]
    public async Task should_send_request_with_correct_method_and_url()
    {
        var expected = _fixture.Create<Anonymous>();

        Dictionary<string, string> meta = CreateMetadata(2);
        Dictionary<string, string> tags = CreateTags(2);

        string filename = _fixture.Create<string>();
        // if request doesnt go to the method and url, the call will return 404 and ApiException
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Post, OperationUrl)
            .WithHeaders(acceptJson: true, meta)
            .WithTags(tags)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(new[] { expected }));

        var file = new FileParameter(GetRandomStream(), filename);

        ObjectManagementClient sut = GetClient(mockHttp);

        var actual = await sut.CreateObjectsAsync(meta, tags, file, CancellationToken.None);
        Assert.NotNull(actual);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task should_add_tags_to_query_string()
    {
        var expected = _fixture.Create<Anonymous>();

        Dictionary<string, string> meta = CreateMetadata(2);
        Dictionary<string, string> tags = CreateTags(2);

        // if request doesnt go to the method and url, the call will return 404 and ApiException
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Post, OperationUrl)
            .WithHeaders(acceptJson: true, meta)
            .WithTags(tags)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(new[] { expected }));

        var file = new FileParameter(GetRandomStream());

        ObjectManagementClient sut = GetClient(mockHttp);

        var actual = await sut.CreateObjectsAsync(meta, tags, file, CancellationToken.None);
        Assert.NotNull(actual);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task should_add_meta_to_headers()
    {
        var expected = _fixture.Create<Anonymous>();

        Dictionary<string, string> meta = CreateMetadata(2);
        Dictionary<string, string> tags = CreateTags(2);

        // if request doesnt go to the method and url, the call will return 404 and ApiException
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Post, OperationUrl)
            .WithHeaders(acceptJson: true, meta)
            .WithTags(tags)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(new[] { expected }));

        var file = new FileParameter(GetRandomStream());

        ObjectManagementClient sut = GetClient(mockHttp);

        var actual = await sut.CreateObjectsAsync(meta, tags, file, CancellationToken.None);
        Assert.NotNull(actual);
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
