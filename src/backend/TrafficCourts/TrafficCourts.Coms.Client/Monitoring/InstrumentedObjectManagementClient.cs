using System;
using System.Security.Cryptography;

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

    public Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("AddMetadata");

        try
        {
            var response = _client.AddMetadataAsync(meta, objId, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("AddTagging");

        try
        {
            var response = _client.AddTaggingAsync(objId, tags, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, Guid? bucketId, FileParameter anyKey, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginCreateObjects();

        try
        {
            var response = _client.CreateObjectsAsync(meta, tags, bucketId, anyKey, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndCreateObjects(operation, exception);
            throw;
        }
    }

    public Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("DeleteMetadata");

        try
        {
            var response = _client.DeleteMetadataAsync(meta, objId, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginDeleteObject();

        try
        {
            var response = _client.DeleteObjectAsync(objId, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndDeleteObject(operation, exception);
            throw;
        }

    }

    public Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("DeleteTagging");

        try
        {
            var response = _client.DeleteTaggingAsync(objId, tags, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<IList<Anonymous2>> FetchMetadataAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? meta, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginFetchMetadata();
        try
        {
            var response = _client.FetchMetadataAsync(ids, meta, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndFetchMetadata(operation, exception);
            throw;
        }
    }

    public Task<IList<Anonymous3>> FetchTagsAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginFetchTags();

        try
        {
            var response = _client.FetchTagsAsync(ids, tags, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndFetchTags(operation, exception);
            throw;
        }
    }

    public Task HeadObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("HeadObject");

        try
        {
            var response = _client.HeadObjectAsync(objId, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<List<DBVersion>> ListObjectVersionAsync(Guid objId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("ListObjectVersion");

        try
        {
            var response = _client.ListObjectVersionAsync(objId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginReadObject();

        try
        {
            var response = _client.ReadObjectAsync(objId, download, expiresIn, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndReadObject(operation, exception);
            throw;
        }
    }

    public Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("ReplaceMetadata");

        try
        {
            var response = _client.ReplaceMetadataAsync(meta, objId, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("ReplaceTagging");

        try
        {
            var response = _client.ReplaceTaggingAsync(objId, tags, versionId, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }

    public Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? ids, Guid? bucketId, string? path, bool? active, bool? deleteMarker, bool? latest, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginSearchObjects();

        try
        {
            var response = _client.SearchObjectsAsync(meta, ids, bucketId, path, active, deleteMarker, latest, @public, mimeType, name, tags, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndSearchObjects(operation, exception);
            throw;
        }
    }

    public Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken)
    {
        using var operation = Instrumentation.BeginOtherOperation("UpdateObject");

        try
        {
            var response = _client.UpdateObjectAsync(meta, objId, tags, anyKey, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            Instrumentation.EndOtherOperation(operation, exception);
            throw;
        }
    }
}
