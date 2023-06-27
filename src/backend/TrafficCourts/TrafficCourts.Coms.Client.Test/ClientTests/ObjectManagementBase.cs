using AutoFixture;
using RichardSzalay.MockHttp;
using System.Net.Http.Headers;

namespace TrafficCourts.Coms.Client.Test.ClientTests;

public abstract class ObjectManagementBase
{
    private const string BaseUrl = "http://localhost/api/v1";
    protected readonly Fixture _fixture = new Fixture();

    protected ObjectManagementClient GetClient(MockHttpMessageHandler mockHttp)
    {
        var httpClient = mockHttp.ToHttpClient();
        ObjectManagementClient client = new(httpClient)
        {
            BaseUrl = BaseUrl
        };

        return client;
    }

    /// <summary>
    /// Creates a <see cref="MemoryStream"/> with some random data containing the bytes of new <see cref="Guid"/>.
    /// </summary>
    /// <returns></returns>
    protected Stream GetRandomStream() => new MemoryStream(Guid.NewGuid().ToByteArray());

    protected IReadOnlyDictionary<string, string> CreateTags(int count = 0)
    {
        Dictionary<string, string> items = new Dictionary<string, string>();
        return Factory.CreateTags(items);
    }

    protected IReadOnlyDictionary<string, string> CreateMetadata(int count = 0)
    {

        Dictionary<string, string> items = new Dictionary<string, string>();
        CreateItems(items, count);
        return Metadata.Create(items);
    }

    private void CreateItems(Dictionary<string, string> items, int count)
    {
        for (int i = 0; i < count; i++)
        {
            string key = _fixture.Create<string>();
            string value = _fixture.Create<string>();
            items.Add(key, value);
        }
    }
}
