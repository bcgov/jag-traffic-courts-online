using RichardSzalay.MockHttp;
using System.Reflection.PortableExecutable;
using TrafficCourts.Coms.Client.Test.ComService;

namespace TrafficCourts.Coms.Client.Test.ComService;

public static class MockHttpExtensions
{
    public static MockedRequest WithTags(this MockedRequest request, IDictionary<string, string> items)
    {
        Dictionary<string, string> values = new();

        foreach (var item in items)
        {
            values.Add($"tagset[{item.Key}]", item.Value);
        }

        request.WithQueryString(values);

        return request;
    }

    public static MockedRequest WithHeaders(this MockedRequest request, BasicAuthenticationHeaderValue? authorization = null, IDictionary<string, string>? items = null)
    {
        Dictionary<string, string> values = new()
        {
            { "Accept", "application/json" }
        };

        if (authorization is not null)
        {
            values.Add("Authorization", $"{authorization.Scheme} {authorization.Parameter}");
        }

        if (items is not null)
        {
            foreach (var item in items)
            {
                values.Add($"x-amz-meta-{item.Key.ToLower()}", item.Value);
            }
        }

        request.WithHeaders(values);

        return request;
    }
}
