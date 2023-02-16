using AutoFixture;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace TrafficCourts.Coms.Client.Test.ClientTests;

public class SearchObjectsAsync : ObjectManagementBase
{
    private const string OperationUrl = "/api/v1/object";

    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task calls_client_with_correct_parameters(Dictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, Dictionary<string, string>? tags)
    {
        List<DBObject> expected = _fixture.CreateMany<DBObject>().ToList();

        // if request doesnt go to the method and url, the call will return 404 and ApiException
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Get, OperationUrl)
            .WithHeaders(acceptJson: true, meta)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expected));

        ObjectManagementClient sut = GetClient(mockHttp);

        await sut.SearchObjectsAsync(meta, objIds, path, active, @public, mimeType, name, tags, CancellationToken.None);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    public static IEnumerable<object?[]> TestCases
    {
        get
        {
            // IDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, IDictionary<string, string>? tags
            yield return new object?[] { null, null, null, null, null, null, null, null };

            yield return new object?[] { null, null, "", false, false, "", "", null };
        }
    }
}