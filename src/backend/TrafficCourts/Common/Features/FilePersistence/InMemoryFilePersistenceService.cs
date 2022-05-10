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

        if (!_cache.TryGetValue(filename, out byte[] buffer))
        {
            throw new FileNotFoundException("File not found", filename);
        }

        var stream = new MemoryStream(buffer);
        return Task.FromResult(stream);
    }

    public override async Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
    {
        var mimeType = await GetMimeTypeAsync(data);
        if (mimeType is null)
        {
            _logger.LogInformation("Could not determine mime type for file, file cannot be saved");
            return string.Empty;
        }

        var filename = GetFileName(mimeType);
        if (filename == string.Empty)
        {
            return string.Empty;
        }

        var buffer = data.ToArray();
        _cache.Set(filename, buffer);

        return filename;
    }
}
