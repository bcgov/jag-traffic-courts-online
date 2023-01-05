using RichardSzalay.MockHttp;

namespace TrafficCourts.Coms.Client.Test.ClientTests;

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

    public static MockedRequest WithPath(this MockedRequest request, string path)
    {
        Dictionary<string, string> values = new()
        {
            { "path", path }
        };

        request.WithQueryString(values);

        return request;
    }

    public static MockedRequest WithHeaders(
        this MockedRequest request, 
        bool acceptJson = true, 
        IDictionary<string, string>? metadata = null)
    {
        Dictionary<string, string> values = new();
        if (acceptJson) values.Add("Accept", "application/json");
        if (metadata is not null)
        {
            foreach (var item in metadata)
            {
                values.Add($"x-amz-meta-{item.Key.ToLower()}", item.Value);
            }
        }

        request.WithHeaders(values);

        return request;
    }
}
