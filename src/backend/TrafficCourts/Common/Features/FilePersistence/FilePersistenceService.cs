using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Diagnostics;
using Winista.Mime;

namespace TrafficCourts.Common.Features.FilePersistence;

public abstract class FilePersistenceService : IFilePersistenceService
{
    protected readonly ILogger<FilePersistenceService> _logger;

    protected FilePersistenceService(ILogger<FilePersistenceService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken);
    public abstract Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken);

    protected string GetFileName(MimeType mimeType)
    {
        string id = Guid.NewGuid().ToString("n");

        // even with an empty buffer, MimeTypes.GetMimeType always seems to return a mime type
        if (mimeType.Extensions is null || mimeType.Extensions.Length == 0)
        {
            _logger.LogDebug("No mime type or extension available");
            return id;
        }

        string extension = mimeType.Extensions[0];
        Debug.Assert(extension is not null);
        return $"{id}.{extension}";
    }
}
