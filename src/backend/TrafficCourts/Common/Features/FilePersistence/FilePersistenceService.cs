using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Diagnostics;

namespace TrafficCourts.Common.Features.FilePersistence;

public abstract class FilePersistenceService : IFilePersistenceService
{
    protected readonly ILogger<FilePersistenceService> _logger;

    protected FilePersistenceService(ILogger<FilePersistenceService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken);
    public abstract Task<string> SaveJsonFileAsync<T>(T data, string filename, CancellationToken cancellationToken);
    public abstract Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken);

    protected string GetFileName(FileMimeType mimeType)
    {
        string id = Guid.NewGuid().ToString("n");
        return $"{id}.{mimeType.Extension}";
    }
}
