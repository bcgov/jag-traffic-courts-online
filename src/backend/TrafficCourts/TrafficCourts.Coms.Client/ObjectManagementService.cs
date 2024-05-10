using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using TrafficCourts.Coms.Client.Monitoring;

namespace TrafficCourts.Coms.Client;

internal partial class ObjectManagementService : IObjectManagementService
{
    private readonly IObjectManagementClient _client;
    private readonly IMemoryStreamFactory _memoryStreamFactory;
    private readonly ILogger<ObjectManagementService> _logger;

    public ObjectManagementService(
        IObjectManagementClient client,
        IMemoryStreamFactory memoryStreamFactory,
        ILogger<ObjectManagementService> logger)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = new InstrumentedObjectManagementClient(client);
        _memoryStreamFactory = memoryStreamFactory ?? throw new ArgumentNullException(nameof(memoryStreamFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a file. On successful creation, the file's <see cref="File.Id"/> will be set.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ObjectManagementServiceException"></exception>
    public async Task<Guid> CreateFileAsync(File file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Data is null)
        {
            throw new ArgumentException("Data is required for creating file", nameof(file));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("create file");
        _logger.LogDebug("Creating file");

        MetadataValidator.Validate(file.Metadata);
        TagValidator.Validate(file.Tags);

        FileParameter parameter = new(file.Data, file.FileName, file.ContentType);

        try
        {
            var created = await _client.CreateObjectsAsync(file.Metadata, file.Tags, bucketId: null, parameter, cancellationToken)
                .ConfigureAwait(false);

            if (created.Count == 0)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                _logger.LogError("Creating file did not return any created objects");
                throw new ObjectManagementServiceException("Could not create file");
            }

            if (created.Count != 1)
            {
                _logger.LogWarning("{Count} created objects were returned, expecting only 1. Returning the first object id.", created.Count);
            }

            Guid id = created[0].Id;
            file.Id = id;
            _logger.LogDebug("Created file {FileId}", id);
            return id;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("creating file", exception);
        }
    }

    public async Task DeleteFileAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("delete file");
        _logger.LogDebug("Deleting file {FileId}", id);

        try
        {
            ResponseObjectDeleted response = await _client.DeleteObjectAsync(id, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogDebug("File deleted {FileId}", id);
        }
        catch (ApiException<ResponseError> exception) when (exception.Result.Detail == "NotFoundError")
        {
            // it is ok if the specific file not found
            _logger.LogDebug("File not found {FileId}", id);
        }
        catch (Exception exception)
        {
            // if the file does not exist, this could return 502 Bad Gateway error
            // see https://github.com/bcgov/common-object-management-service/issues/89
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("deleting file", exception);
        }
    }

    public async Task<File> GetFileAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot get file on empty object id", nameof(id));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("get file");
        _logger.LogDebug("Getting file {FileId}", id);

        try
        {            
            FileResponse response = await _client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            string? contentType = GetHeader(response.Headers, "Content-Type");
            string? fileName = response.GetFilename();
            if (fileName is null)
            {
                LogNoFilenameForObject(id);
            }

            // get the metadata and tags for this object
            Dictionary<Guid, IList<DBMetadataKeyValue>> metadataValues = await GetMetadataAsync(id, cancellationToken);
            IReadOnlyDictionary<string, string>? metadata = GetMetadataForId(id, metadataValues);

            Dictionary<Guid, IList<DBTagKeyValue>> tagValues = await GetTagsAsync(id, cancellationToken);
            IReadOnlyDictionary<string, string>? tags = GetTagsForId(id, tagValues);

            // make a copy of the stream because the FileResponse will dispose of the stream
            var stream = _memoryStreamFactory.GetStream();
            await response.Stream.CopyToAsync(stream, cancellationToken);

            var file = new File(id, stream, fileName, contentType, metadata, tags);
            return file;
        }
        catch (ApiException exception) when (exception.StatusCode == /*StatusCodes.Status404NotFound*/ 404) // StatusCodes Class requires Package: Microsoft.AspNetCore.App.Ref v7.0.5, so use constant
        {
            LogFileNotFound(exception, id);
            throw new FileNotFoundException($"File with {id} not found");
        }
        catch (ApiException exception)
        {
            // Error calling the COMS service
            // - ReadObjectAsync failed, or
            // - GetMetadataAsync failed, or
            // - GetTagsAsync failed
            activity?.SetStatus(ActivityStatusCode.Error);
            LogGetFileError(exception, id);
            throw new ObjectManagementServiceException("API error getting file", exception);
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("getting file", exception);
        }
    }

    public async Task<IList<FileSearchResult>> FileSearchAsync(FileSearchParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        using Activity? activity = Diagnostics.Source.StartActivity("file search");
        _logger.LogDebug("Searching for files");

        MetadataValidator.Validate(parameters.Metadata);
        TagValidator.Validate(parameters.Tags);

        try
        {
            List<DBObject> files = await _client.SearchObjectsAsync(
                parameters.Metadata,
                parameters.Ids,
                parameters.BucketId,
                parameters.Path,
                parameters.Active,
                parameters.DeleteMarker,
                parameters.Latest,
                parameters.Public,
                parameters.MimeType,
                parameters.Name,
                parameters.Tags,
                cancellationToken).ConfigureAwait(false);

            if (files.Count == 0)
            {
                _logger.LogDebug("No files found");
                return Array.Empty<FileSearchResult>();
            }

            _logger.LogDebug("Found {Count} files", files.Count);

            // fetch all of the metadata for found files at once
            var ids = files.Select(_ => _.Id).ToList();
            var metadataValues = await GetMetadataAsync(ids, cancellationToken);
            var tagValues = await GetTagsAsync(ids, cancellationToken);

            // allocate list with the correct size based on the number of found files
            List<FileSearchResult> results = new(files.Count);

            foreach (var file in files)
            {
                // get this files metadata and tags
                IReadOnlyDictionary<string, string> metadata = GetMetadataForId(file.Id, metadataValues);
                IReadOnlyDictionary<string, string> tags = GetTagsForId(file.Id, tagValues);

                // filename will be returned in the metadata pairs
                string filename = GetFileName(file.Id, metadataValues);

                // creat end user search results
                var result = new FileSearchResult(
                    file.Id,
                    filename,
                    file.Path,
                    file.Active,
                    file.Public,
                    file.CreatedBy,
                    file.CreatedAt,
                    file.UpdatedBy ?? Guid.Empty,
                    file.UpdatedAt,
                    metadata,
                    tags);

                results.Add(result);
            }

            results = FilterSearchResults(results, parameters);

            return results;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("searching files", exception);
        }
    }

    public async Task UpdateFileAsync(Guid id, File file, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(file);

        if (file.Data is null)
        {
            throw new ArgumentException("Data is required for updating a file", nameof(file));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("update file");
        _logger.LogDebug("Updating file");


        MetadataValidator.Validate(file.Metadata);
        TagValidator.Validate(file.Tags);

        FileParameter parameter = new(file.Data, file.FileName, file.ContentType);

        try
        {
            await _client.UpdateObjectAsync(file.Metadata, id, file.Tags, parameter, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("updating file", exception);
        }
    }

    public async Task ReplaceMetadataAsync(Guid id, IReadOnlyDictionary<string, string> meta, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(meta);

        using Activity? activity = Diagnostics.Source.StartActivity("replace metadata");

        try
        {
            await _client.ReplaceMetadataAsync(meta, id, versionId: null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("replacing metadata", exception);
        }

        throw new ArgumentException("Cannot add tags to empty object id", nameof(id));
    }

    public async Task SetTagsAsync(Guid id, IReadOnlyDictionary<string, string> tags, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot set tags on file with empty object id", nameof(id));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("set tags");
        try
        {
            await _client.ReplaceTaggingAsync(id, tags, null, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("replacing tags", exception);
        }
    }

    public async Task AddTagsAsync(Guid id, IReadOnlyDictionary<string, string> tags, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot add tags on file with empty object id", nameof(id));
        }

        using Activity? activity = Diagnostics.Source.StartActivity("add tags");
        try
        {
            await _client.AddTaggingAsync(id, tags, null, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw ExceptionHandler("adding tags", exception);
        }
    }

    /// <summary>
    /// Tries to get the named header.
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string? GetHeader(IReadOnlyDictionary<string, IEnumerable<string>> headers, string name)
    {
        string? value = null;
        if (headers.TryGetValue(name, out IEnumerable<string>? values))
        {
            if (values is not null)
            {
                value = values.FirstOrDefault();
            }
        }

        return value;
    }

    /// <summary>
    /// Gets the filename from the metadata values. Do not use this function to get 
    /// the filename from headers.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    private static string GetFileName(Guid id, Dictionary<Guid, IList<DBMetadataKeyValue>> values)
    {
        Debug.Assert(values != null);

        if (!values.TryGetValue(id, out var items))
        {
            return string.Empty;
        }

        return items.FirstOrDefault(item => item.Key == Metadata.Keys.Name)?.Value ?? string.Empty;
    }

    /// <summary>
    /// COMS does like search, we want to do equals comparison.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static List<FileSearchResult> FilterSearchResults(List<FileSearchResult> results, FileSearchParameters parameters)
    {
        Debug.Assert(results != null);
        Debug.Assert(parameters != null);

        results = results
            .Where(result => TagsMatch(result, parameters) && MetadataMatch(result, parameters))
            .ToList();

        return results;
    }

    /// <summary>
    /// Checks to ensure all of the tags search properties for match on the result.
    /// </summary>
    /// <returns>All of the searched tags match on the result</returns>
    private static bool TagsMatch(FileSearchResult result, FileSearchParameters parameters)
    {
        Debug.Assert(result != null);
        Debug.Assert(parameters != null);

        foreach (var tag in parameters.Tags)
        {
            if (result.Tags.TryGetValue(tag.Key, out string? value) && tag.Value != value)
            {
                return false; // tag value does not match
            }
        }

        return true;
    }

    /// <summary>
    /// Checks to ensure all of the metadata search properties for match on the result.
    /// </summary>
    /// <returns>All of the searched metadata match on the result</returns>
    private static bool MetadataMatch(FileSearchResult result, FileSearchParameters parameters)
    {
        Debug.Assert(result != null);
        Debug.Assert(parameters != null);

        foreach (var metadata in parameters.Metadata)
        {
            if (result.Metadata.TryGetValue(metadata.Key, out string? value) && metadata.Value != value)
            {
                return false; // metadata value does not match
            }
        }

        return true;
    }


    private async Task<Dictionary<Guid, IList<DBMetadataKeyValue>>> GetMetadataAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        Debug.Assert(ids != null);

        try
        {
            IList<Anonymous2> response = await _client.FetchMetadataAsync(ids, null, cancellationToken).ConfigureAwait(false);

            var byObjectId = response.ToDictionary(_ => _.ObjectId, _ => _.Metadata);
            return byObjectId;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("fetch metadata", exception);
        }
    }

    private static IReadOnlyDictionary<string, string> GetMetadataForId(Guid id, Dictionary<Guid, IList<DBMetadataKeyValue>> values)
    {
        Debug.Assert(values != null);

        if (!values.TryGetValue(id, out var items))
        {
            return new Dictionary<string, string>();
        }

        return items.ToDictionary(_ => _.Key, _ =>  _.Value);
    }

    private async Task<Dictionary<Guid, IList<DBMetadataKeyValue>>> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetMetadataAsync(new[] { id }, cancellationToken).ConfigureAwait(false);
    }

    private static IReadOnlyDictionary<string, string> GetTagsForId(Guid id, Dictionary<Guid, IList<DBTagKeyValue>> values)
    {
        Debug.Assert(values != null);

        if (!values.TryGetValue(id, out var value))
        {
            return new Dictionary<string, string>();
        }

        return value.ToDictionary(_ => _.Key, _ => _.Value);
    }

    private async Task<Dictionary<Guid, IList<DBTagKeyValue>>> GetTagsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetTagsAsync(new[] { id }, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Dictionary<Guid, IList<DBTagKeyValue>>> GetTagsAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        var response = await _client.FetchTagsAsync(ids, null, cancellationToken).ConfigureAwait(false);
        Dictionary<Guid, IList<DBTagKeyValue>> byObjectId = response.ToDictionary(_ => _.ObjectId, _ => _.Tagset);
        return byObjectId;
    }

    private Exception ExceptionHandler(string operation, Exception exception)
    {
        // TODO: this should be enhaced to provide better error troubleshooting

        if (exception is ApiException apiException)
        {
            LogComsApiError(apiException, operation);
            return new ObjectManagementServiceException($"API error {operation}", exception);
        }

        LogComsOtherError(exception, operation);
        return new ObjectManagementServiceException($"Other error {operation}", exception);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "No filename found for file", EventName = "NoFilenameForObject")]
    private partial void LogNoFilenameForObject(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordObjectId), OmitReferenceName = true)]
        Guid id);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "File not found", EventName = "FileNotFound")]
    private partial void LogFileNotFound(
        ApiException exception,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordObjectId), OmitReferenceName = true)]
        Guid id);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Could not get file by id", EventName = "GetFileError")]
    private partial void LogGetFileError(
    ApiException exception,
    [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordObjectId), OmitReferenceName = true)]
        Guid id);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "API error accessing COMS", EventName = "ComsApiError")]
    private partial void LogComsApiError(
        ApiException exception,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordOperation), OmitReferenceName = true)]
        string operation);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Other error accessing COMS", EventName = "ComsOtherError")]
    private partial void LogComsOtherError(
        Exception exception,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordOperation), OmitReferenceName = true)]
        string operation);

}


public static class TagProvider
{
    public static void RecordObjectId(ITagCollector collector, Guid id)
    {
        collector.Add("ObjectId", id);
    }

    public static void RecordOperation(ITagCollector collector, string operation)
    {
        collector.Add("Operation", operation);
    }
}
