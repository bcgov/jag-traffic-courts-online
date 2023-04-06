namespace TrafficCourts.Coms.Client.Monitoring;

internal class InstrumentedObjectManagementClient : IObjectManagementClient
{
    private readonly IObjectManagementClient _client;

    public InstrumentedObjectManagementClient(IObjectManagementClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public string BaseUrl { get => _client.BaseUrl; set => _client.BaseUrl = value; }
    public bool ReadResponseAsString { get => _client.ReadResponseAsString; set => _client.ReadResponseAsString = value; }

    public async Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.AddMetadataAsync));

        try
        {
            await _client.AddMetadataAsync(meta, objId, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.AddTaggingAsync));

        try
        {
            await _client.AddTaggingAsync(objId, tags, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, Guid? bucketId, FileParameter anyKey, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.CreateObjectsAsync));

        try
        {
            var response = await _client.CreateObjectsAsync(meta, tags, bucketId, anyKey, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.DeleteMetadataAsync));

        try
        {
            await _client.DeleteMetadataAsync(meta, objId, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.DeleteObjectAsync));

        try
        {
            var response = await _client.DeleteObjectAsync(objId, versionId, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }

    }

    public async Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.DeleteTaggingAsync));

        try
        {
            await _client.DeleteTaggingAsync(objId, tags, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<IList<Anonymous2>> FetchMetadataAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? meta, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.FetchMetadataAsync));
        try
        {
            var response = await _client.FetchMetadataAsync(ids, meta, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<IList<Anonymous3>> FetchTagsAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.FetchTagsAsync));

        try
        {
            var response = await _client.FetchTagsAsync(ids, tags, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task HeadObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.HeadObjectAsync));

        try
        {
            await _client.HeadObjectAsync(objId, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<List<DBVersion>> ListObjectVersionAsync(Guid objId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.ListObjectVersionAsync));

        try
        {
            var response = await _client.ListObjectVersionAsync(objId, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.ReadObjectAsync));

        try
        {
            var response = await _client.ReadObjectAsync(objId, download, expiresIn, versionId, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.ReplaceMetadataAsync));

        try
        {
            await _client.ReplaceMetadataAsync(meta, objId, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.ReplaceTaggingAsync));

        try
        {
            await _client.ReplaceTaggingAsync(objId, tags, versionId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? ids, Guid? bucketId, string? path, bool? active, bool? deleteMarker, bool? latest, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.SearchObjectsAsync));

        try
        {
            var response = await _client.SearchObjectsAsync(meta, ids, bucketId, path, active, deleteMarker, latest, @public, mimeType, name, tags, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }

    public async Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOperation(nameof(IObjectManagementClient.UpdateObjectAsync));

        try
        {
            var response = await _client.UpdateObjectAsync(meta, objId, tags, anyKey, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOperation(operation, exception);
            throw;
        }
    }
}
