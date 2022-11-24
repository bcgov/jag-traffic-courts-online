using AutoFixture;
using RichardSzalay.MockHttp;
using System.Net;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Coms.Client.Test.ComService;

public class HeadObject : ObjectManagementBase
{
    [Fact]
    public async Task should_send_request_with_correct_method_and_url()
    {
        Guid id = _fixture.Create<Guid>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Head, $"/api/v1/object/{id}")
            .Respond(HttpStatusCode.NoContent);

        HttpClient client = mockHttp.ToHttpClient();

        ObjectManagementClient sut = new ObjectManagementClient(client);
        sut.BaseUrl = "http://localhost/api/v1";

        await sut.HeadObjectAsync(id, null, CancellationToken.None);
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
