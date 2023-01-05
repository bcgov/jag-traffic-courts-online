using Microsoft.Extensions.Logging;

namespace TrafficCourts.Coms.Client;

internal class ObjectManagementService : IObjectManagementService
{
    private readonly IObjectManagementClient _client;
    private readonly IMemoryStreamFactory _memoryStreamFactory;
    private readonly ILogger<ObjectManagementService> _logger;

    public ObjectManagementService(IObjectManagementClient client, IMemoryStreamFactory memoryStreamFactory, ILogger<ObjectManagementService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _memoryStreamFactory = memoryStreamFactory ?? throw new ArgumentNullException(nameof(memoryStreamFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> CreateFileAsync(File file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Data is null)
        {
            throw new ArgumentException("Data is required for creating file", nameof(file));
        }

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

            return created[0].Id;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("creating file", exception);
        }
    }

    public async Task DeleteFileAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            ResponseObjectDeleted response = await _client.DeleteObjectAsync(id, versionId: null, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // if the file does not exist, this could return 502 Bad Gateway error
            // see https://github.com/bcgov/common-object-management-service/issues/89
            throw ExceptionHandler("deleting file", exception);
        }
    }

    public async Task<File> GetFileAsync(Guid id, bool includeTags, CancellationToken cancellationToken)
    {
        try
        {
            FileResponse response = await _client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            string? contentType = GetHeader(response.Headers, "Content-Type");
            string? fileName = GetHeader(response.Headers, "x-amz-meta-name");

            IDictionary<string, string>? metadata = GetMetadata(response);
            IDictionary<string, string>? tags = null;

            if (includeTags)
            {
                tags = await GetTagsAsync(id, cancellationToken);
            }

            // make a copy of the stream because the FileResponse will dispose of the stream
            var stream = _memoryStreamFactory.GetStream();
            await response.Stream.CopyToAsync(stream, cancellationToken);

            var file = new File(stream, fileName, contentType, metadata, tags);
            return file;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("getting file", exception);
        }
    }

    public async Task<List<FileSearchResult>> FileSearchAsync(FileSearchParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        MetadataValidator.Validate(parameters.Metadata);
        TagValidator.Validate(parameters.Tags);

        try
        {
            List<DBObject> files = await _client.SearchObjectsAsync(
                parameters.Metadata,
                parameters.Ids,
                parameters.Path,
                parameters.Active,
                parameters.Public,
                parameters.MimeType,
                parameters.Name,
                parameters.Tags,
                cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Found {Count} files", files.Count);

            List<FileSearchResult> results = new(files.Count);

            foreach (var file in files)
            {
                // fetch the meta data and tags
                var metadata = await GetMetadataAsync(file.Id, cancellationToken);
                var tags = await GetTagsAsync(file.Id, cancellationToken);

                var result = new FileSearchResult(
                    file.Id,
                    file.Path,
                    file.Active,
                    file.Public,
                    file.CreatedBy,
                    file.CreatedAt,
                    file.UpdatedBy,
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
        ArgumentNullException.ThrowIfNull(file);
        
        if (file.Data is null)
        {
            throw new ArgumentException("Data is required for updating a file", nameof(file));
        }

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

    private async Task<IDictionary<string, string>> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            // use the url mode to avoid downloading the file contents
            FileResponse response = await _client.ReadObjectAsync(id, DownloadMode.Url, expiresIn: null, versionId: null, cancellationToken)
                .ConfigureAwait(false);

            IDictionary<string, string> metadata = GetMetadata(response);
            return metadata;
        }
        catch (Exception exception)
        {
            throw ExceptionHandler("fetch metadata", exception);
        }
    }

    private IDictionary<string, string> GetMetadata(FileResponse response)
    {
        var metadata = new Dictionary<string,string>();

        bool IsMetadataHeader(KeyValuePair<string, IEnumerable<string>> header)
        {
            return header.Key.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase);
        }

        foreach (var header in response.Headers.Where(IsMetadataHeader))
        {
            // header is KeyValuePair<string, IEnumerable<string>>
            // extract the meta data key without the prefix
            var key = header.Key["x-amz-meta-".Length..];

            if (key == "id" || key == "name")
            {                
                continue; // skip internal meta data properties
            }

            var value = header.Value.FirstOrDefault() ?? string.Empty;
            metadata.Add(key, value);
        }

        return metadata;
    }

    private Task<IDictionary<string, string>> GetTagsAsync(Guid id, CancellationToken cancellationToken)
    {
        // TODO: need to determine how to get tags, see: https://github.com/bcgov/common-object-management-service/issues/93
        // May need to query the database directly in the interim
        IDictionary<string, string> tags = Factory.CreateTags();
        return Task.FromResult(tags);
    }

    private Exception ExceptionHandler(string operation, Exception exception)
    {
        if (exception is ApiException)
        {
            _logger.LogError(exception, "API error {operation}", operation);
            return new ObjectManagementServiceException($"API error {operation}", exception);
        }

        _logger.LogError(exception, "Other error {operation}", operation);
        return new ObjectManagementServiceException($"Other error {operation}", exception);
    }
}
