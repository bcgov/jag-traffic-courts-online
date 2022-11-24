using AutoFixture;
using RichardSzalay.MockHttp;

namespace TrafficCourts.Coms.Client.Test.ComService;

public abstract class ObjectManagementBase
{
    private const string BaseUrl = "http://localhost/api/v1";
    protected readonly Fixture _fixture = new Fixture();

    protected ObjectManagementClient GetClient(MockHttpMessageHandler mockHttp)
    {
        ObjectManagementClient cient = new(mockHttp.ToHttpClient())
        {
            BaseUrl = BaseUrl
        };

        return cient;
    }
}
