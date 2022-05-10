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
    public abstract Task DeleteFileAsync(string filename, CancellationToken cancellationToken);

    protected async Task<MimeType> GetMimeTypeAsync(Stream data)
    {
        var pool = ArrayPool<byte>.Shared;
        // get a buffer to read the first 1K of the file stream
        // any of our images should be at least 1KB be.
        var buffer = pool.Rent(1024);

        try
        {
            int count = await data.ReadAsync(buffer, 0, buffer.Length);
            var mimeTypes = new MimeTypes();
            MimeType mimeType = mimeTypes.GetMimeType(buffer);
            return mimeType;
        }
        finally
        {
            data.Seek(0L, SeekOrigin.Begin); // reset the memory stream to the beginning
            pool.Return(buffer);
        }
    }

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
