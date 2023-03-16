using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TrafficCourts.Coms.Client;

internal class ObjectManagementService : IObjectManagementService
{
    private readonly IObjectManagementClient _client;
    private readonly IMemoryStreamFactory _memoryStreamFactory;
    private readonly ILogger<ObjectManagementService> _logger;

    public ObjectManagementService(
        IObjectManagementClient client, 
        IMemoryStreamFactory memoryStreamFactory, 
        ILogger<ObjectManagementService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
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
            throw ExceptionHandler("creating file", exception);
        }
    }

    public async Task DeleteFileAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

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
            throw ExceptionHandler("deleting file", exception);
        }
    }

    public async Task<File> GetFileAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot get file on empty object id", nameof(id));
        }

        _logger.LogDebug("Getting file {FileId}", id);

        try
        {
            FileResponse response = await _client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            string? contentType = GetHeader(response.Headers, "Content-Type");
            string? fileName = GetHeader(response.Headers, "x-amz-meta-name");

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
        catch (ApiException exception) when (exception.StatusCode == StatusCodes.Status404NotFound)
        {
            _logger.LogInformation(exception, "File not found with id {FileId}", id);
            throw new FileNotFoundException($"File with {id} not found");
        }
        catch (ApiException exception)
        {
            // Error calling the COMS service
            // - ReadObjectAsync failed, or
            // - GetMetadataAsync failed, or
            // - GetTagsAsync failed
            _logger.LogInformation(exception, "Could not get file by id, FileId={FileId}", id);
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

        _logger.LogDebug("Searching for files");

        MetadataValidator.Validate(parameters.Metadata);
        TagValidator.Validate(parameters.Tags);

        // 2023-03-15 LDAME if the following line fails it may be due to the generated code of SearchObjectsAsync failing on return code 201
        // the generated code recognizes 200 as success but does not recognize 201 as success
        // this may be fixed in a soon upcoming release of COMS
        // otherwise update the generated code to if (status_ == 200 || status_ == 201)
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

        try
        {
            await _client.ReplaceMetadataAsync(meta, id, versionId: null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
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

        try
        {
            await _client.ReplaceTaggingAsync(id, tags, null, cancellationToken);
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("replacing tags", exception);
        }
    }

    public async Task AddTagsAsync(Guid id, IReadOnlyDictionary<string, string> tags, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot add tags on file with empty object id", nameof(id));
        }

        try
        {
            await _client.AddTaggingAsync(id, tags, null, cancellationToken);
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("adding tags", exception);
        }
    }

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

    private string GetFileName(Guid id, Dictionary<Guid, IList<DBMetadataKeyValue>> values)
    {
        if (!values.TryGetValue(id, out var items))
        {
            return string.Empty;
        }

        foreach (var item in items)
        {
            if (Metadata.IsName(item.Key))
            {
                return item.Value;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// COMS does like search, we want to do equals comparison.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private List<FileSearchResult> FilterSearchResults(List<FileSearchResult> results, FileSearchParameters parameters)
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

        return items
            .Where(_ => !Metadata.IsInternal(_.Key))
            .ToDictionary(_ => _.Key, _ =>  _.Value);
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

        Debug.Assert(ids != null);

        try
        {
            IList<Anonymous3> response = await _client.FetchTagsAsync(ids, null, cancellationToken).ConfigureAwait(false);

            var byObjectId = response.ToDictionary(_ => _.ObjectId, _ => _.Tagset);
            return byObjectId;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("fetch tags", exception);
        }
    }

    private Exception ExceptionHandler(string operation, Exception exception)
    {
        // TODO: this should be enhaced to provide better error troubleshooting

        if (exception is ApiException)
        {
            _logger.LogError(exception, "API error {operation}", operation);
            return new ObjectManagementServiceException($"API error {operation}", exception);
        }

        _logger.LogError(exception, "Other error {operation}", operation);
        return new ObjectManagementServiceException($"Other error {operation}", exception);
    }
}
