using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace TrafficCourts.Common.Features.FilePersistence;

/// <summary>
/// A file persistence service that can be used in development that only saves data in memory.
/// </summary>
public class InMemoryFilePersistenceService : FilePersistenceService
{
    private readonly IMemoryCache _cache;

    public InMemoryFilePersistenceService(IMemoryCache cache, ILogger<InMemoryFilePersistenceService> logger)
        : base(logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public override Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filename);

        if (!_cache.TryGetValue(filename, out byte[]? buffer))
        {
            throw new FileNotFoundException("File not found", filename);
        }

        if (buffer is null)
        {
            throw new FileNotFoundException("File not found", filename);
        }

        var stream = new MemoryStream(buffer);
        return Task.FromResult(stream);
    }

    public override Task<T?> GetJsonDataAsync<T>(string filename, CancellationToken cancellationToken) where T : class
    {
        throw new NotImplementedException();
    }

    public override Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
    {
        var mimeType = data.GetMimeType();
        if (mimeType is null)
        {
            _logger.LogInformation("Could not determine mime type for file, file cannot be saved");
            return Task.FromResult(string.Empty);
        }

        var filename = GetFileName(mimeType);

        var buffer = data.ToArray();
        _cache.Set(filename, buffer);

        return Task.FromResult(string.Empty);
    }

    public override Task<string> SaveJsonFileAsync<T>(T data, string filename, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
