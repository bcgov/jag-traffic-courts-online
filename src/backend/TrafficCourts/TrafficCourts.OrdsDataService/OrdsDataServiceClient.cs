using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Web;

namespace TrafficCourts.OrdsDataService;

internal class OrdsDataServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IOrdsDataServiceOperationMetrics _metrics;
    private bool _readContentAsString = false;

    public OrdsDataServiceClient(HttpClient httpClient, IOrdsDataServiceOperationMetrics metrics)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _metrics = metrics;
    }

    public async Task<T?> GetAsync<T>(
        string path,
        IReadOnlyDictionary<string, string>? parameters,
        JsonTypeInfo<T> jsonTypeInfo,
        ETagCache cache,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);

        using var operation = _metrics.BeginOperation(path);
        operation.AddTag("method", "GET");

        try
        {
            // build the path with query string parameters
            Debug.Assert(_httpClient.BaseAddress != null);
            path = _httpClient.BaseAddress!.AbsolutePath + GetPath(path, parameters);

            // create our request and set the request options for caching

            HttpRequestMessage request = new(HttpMethod.Get, path);
            // add the cache policy to the request options
            request.Options.Set(new HttpRequestOptionsKey<ETagCache>(nameof(ETagCache)), cache);

            // send the request
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            T? result = await ReadContentAsync(response, jsonTypeInfo, cancellationToken);

            return result;

        }
        catch (Exception exception)
        {
            operation.Error(exception);
            throw;
        }
    }

    /// <summary>
    /// Post a series of database operations
    /// </summary>
    /// <param name="path"></param>
    /// <param name="operations">One or more operations that should be executed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task PostAsync(string path, BatchDatabaseOperation operations, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(operations);

        using var postOperation = _metrics.BeginOperation(path);
        postOperation.AddTag("method", "POST");
        postOperation.AddTag("operation-count", operations.Count);

        try
        {
            // build the path with query string parameters
            Debug.Assert(_httpClient.BaseAddress != null);
            path = _httpClient.BaseAddress!.AbsolutePath + path;

            HttpRequestMessage request = new(HttpMethod.Post, path);

            request.Content = new StringContent(JsonSerializer.Serialize(operations), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

        }
        catch (Exception exception)
        {
            postOperation.Error(exception);
            throw;
        }
    }


    private async Task<T?> ReadContentAsync<T>(HttpResponseMessage response, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
    {
        T? result;

        if (_readContentAsString)
        {
            // used for debugging
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            result = JsonSerializer.Deserialize(content, jsonTypeInfo);
        }
        else
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            result = await JsonSerializer.DeserializeAsync<T>(responseStream, jsonTypeInfo, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Builds the path with the query string parameters
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static string GetPath(string path, IReadOnlyDictionary<string, string>? parameters)
    {
        if (parameters != null && parameters.Count > 0)
        {
            var query = BuildQuery(parameters);
            char separator = path.Contains('?') ? '&' : '?';
            path = $"{path}{separator}{query}";
        }

        return path;
    }

    /// <summary>
    /// Builds the query string with encoded key values
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static NameValueCollection BuildQuery(IReadOnlyDictionary<string, string> parameters)
    {
        // by using HttpUtility.ParseQueryString, the resulting query string will be properly encoded
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

        foreach (var parameter in parameters.OrderBy(_ => _.Key))
        {
            query[parameter.Key] = parameter.Value;
        }

        return query;
    }
}
