#define USE_COMS_REPOSITORY

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrafficCourts.Coms.Client.Data;

namespace TrafficCourts.Coms.Client;

internal class ObjectManagementService : IObjectManagementService
{
    private readonly IObjectManagementClient _client;
    private readonly IObjectManagementRepository _repository;
    private readonly IMemoryStreamFactory _memoryStreamFactory;
    private readonly ILogger<ObjectManagementService> _logger;

    public ObjectManagementService(
        IObjectManagementClient client, 
        IObjectManagementRepository repository,
        IMemoryStreamFactory memoryStreamFactory, 
        ILogger<ObjectManagementService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
            var created = await _client.CreateObjectsAsync(file.Metadata, file.Tags, parameter, cancellationToken)
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
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

        _logger.LogDebug("Getting file {FileId}", id);

        try
        {
            FileResponse response = await _client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            string? contentType = GetHeader(response.Headers, "Content-Type");
            string? fileName = GetHeader(response.Headers, "x-amz-meta-name");

            var metadataValues = await GetMetadataAsync(id, cancellationToken);
            IDictionary<string, string>? metadata = Client.Metadata.Create(metadataValues[id]);

            IDictionary<string, string>? tags = await GetTagsAsync(id, cancellationToken);

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

        try
        {
            List<DBObject> files = await _client.SearchObjectsAsync(
                parameters.Metadata,
                parameters.Ids,
                parameters.Path,
                parameters.Active,
                false, // deleteMarker
                true,  // latest
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
            var foundIds = files.Select(_ => _.Id).ToList();
            var objectMetadataById = await GetMetadataAsync(foundIds, cancellationToken);
            
            // allocate list with the correct size based on the number of found files
            List<FileSearchResult> results = new(files.Count);

            foreach (var file in files)
            {
                IEnumerable<KeyValuePair<string, string>> pairs = objectMetadataById[file.Id];

                // filename will be returned in the metadata pairs
                string filename = GetFileName(pairs);

                // create user metadata without the internal metadata
                var metadata = Metadata.Create(pairs.Where(Metadata.IsNotInternal));

                // get tags for this file
                var tags = await GetTagsAsync(file.Id, cancellationToken);

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

    private string GetFileName(IEnumerable<KeyValuePair<string, string>> metadata)
    {
        foreach (var item in metadata)
        {
            if (Metadata.IsName(item))
            {
                return item.Value;
            }
        }

        return string.Empty;
    }

    private async Task<ILookup<Guid, KeyValuePair<string, string>>> GetMetadataAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
#if false // USE_COMS_REPOSITORY
        try
        {
            IList<ObjectMetadata> objectsMetadata = await _client.GetObjectMetadataAsync(ids, cancellationToken).ConfigureAwait(false);
            
            // not great we have to flatten out the data, but it makes using a ILookup easier to process on the caller
            // lookup is like a Dictionary<key,IEnumerable<value>> but if the key does not exist, the enumerable is empty
            var lookup = Flatten(objectsMetadata).ToLookup(_ => _.Item1, _ => _.Item2);
            return lookup;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("fetch metadata", exception);
        }
#else
        var lookup = GetMetadataFromRepository(ids);
        return lookup;
#endif
    }

    private ILookup<Guid, KeyValuePair<string, string>> GetMetadataFromRepository(IList<Guid> ids)
    {
        try
        {
            // hack to make the work around look like non-workaround solution
            List<(Guid ObjectId, KeyValuePair<string, string> Item)> values = new();

            foreach (var id in ids)
            {
                var items = _repository.GetObjectMetadata(id);
                foreach (var item in items)
                {
                    values.Add((id, item));
                }
            }

            var lookup = values.ToLookup(_ => _.ObjectId, _ => _.Item);
            return lookup;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("fetch metadata", exception);
        }
    }

    private async Task<ILookup<Guid, KeyValuePair<string, string>>> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetMetadataAsync(new[] { id }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Flattens the collection of object metadata into something that can be converted to a lookup
    /// </summary>
    private IEnumerable<Tuple<Guid, KeyValuePair<string, string>>> Flatten(IList<ObjectMetadata> items)
    {
        foreach (var objectItem in items.Where(_ => _.Metadata != null && _.Metadata.Count > 0))
        {
            foreach (var metaDataItem in objectItem.Metadata)
            {
                yield return Tuple.Create(objectItem.Id, new KeyValuePair<string, string>(metaDataItem.Key, metaDataItem.Value));
            }
        }
    }

    private Task<IDictionary<string, string>> GetTagsAsync(Guid id, CancellationToken cancellationToken)
    {
#if false // USE_COMS_REPOSITORY
        // TODO: need to determine how to get tags, see: https://github.com/bcgov/common-object-management-service/issues/93
        // May need to query the database directly in the interim
        IDictionary<string, string> tags = Factory.CreateTags();
        return Task.FromResult(tags);
#else
        var items = _repository.GetObjectTags(id);
        IDictionary<string, string> tags = Factory.CreateTags(items);
        return Task.FromResult(tags);
#endif
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
