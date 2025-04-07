using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization.Metadata;

namespace TrafficCourts.OrdsDataService;

internal abstract partial class OrdsRepository<TOrdsRepository> 
    where TOrdsRepository : OrdsRepository<TOrdsRepository>
{
    private readonly OrdsDataServiceClient _client;
    private readonly string _path;
    protected readonly ILogger<TOrdsRepository> _logger;

    protected OrdsRepository(OrdsDataServiceClient client, string path, ILogger<TOrdsRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(logger);

        _client = client;
        _path = path;
        _logger = logger;
    }

    protected async Task<OrdsDataServicePagedCollectionResponse<T>> GetPagedListAsync<T>(
        IReadOnlyDictionary<string, string>? parameters,
        JsonTypeInfo<OrdsDataServicePagedCollectionResponse<T>> jsonTypeInfo,
        ETagCache policy,
        CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync(
            _path,
            parameters,
            jsonTypeInfo,
            policy,
            cancellationToken);

        if (response is null || (response.Rows is null && response.Errors is null))
        {
            throw new OrdsException("No data rows and no error rows, this should not happen");
        }

        if (response.Errors is not null)
        {
            OrdsDataServiceError error = response.Errors.Single();
            LogOrdsError(_path, error);
            throw new OrdsErrorException(error);
        }

        return response;
    }

    protected async Task<OrdsDataServiceCollectionResponse<T>> GetListAsync<T>(
        IReadOnlyDictionary<string, string>? parameters,
        JsonTypeInfo<OrdsDataServiceCollectionResponse<T>> jsonTypeInfo,
        ETagCache policy,
        CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync(
            _path,
            parameters,
            jsonTypeInfo,
            policy,
            cancellationToken);

        if (response is null || (response.Rows is null && response.Errors is null))
        {
            throw new OrdsException("No data rows and no error rows, this should not happen");
        }

        if (response.Errors is not null)
        {
            OrdsDataServiceError error = response.Errors.Single();
            LogOrdsError(_path, error);
            throw new OrdsErrorException(error);
        }

        return response;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Error getting data")]
    private partial void LogOrdsError(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.Record), OmitReferenceName = true)]
        string path,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.Record), OmitReferenceName = true)]
        OrdsDataServiceError error);
}


[Serializable]
public class OrdsException : Exception
{
    public OrdsException(string message) : base(message) { }
    public OrdsException(string message, Exception inner) : base(message, inner) { }
}


[Serializable]
public class OrdsErrorException : OrdsException
{
    public OrdsErrorException(OrdsDataServiceError error) : base(error.ErrorMessage)
    {
    }
}

public static class TagProvider
{
    public static void Record(ITagCollector collector, OrdsDataServiceError error)
    {
        collector.Add("OrdsErrorCode", error.ErrorCode);
        collector.Add("OrdsErrorMessage", error.ErrorMessage);
        collector.Add("OrdsErrorStack", error.ErrorStack);
    }

    public static void Record(ITagCollector collector, string path)
    {
        collector.Add("OrdsPath", path);
    }
}