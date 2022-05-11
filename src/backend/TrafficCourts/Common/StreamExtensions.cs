using System.Buffers;
using Winista.Mime;

namespace System.IO;

public static class StreamExtensions
{
    /// <summary>
    /// Scans the first 1K of a data stream looking for the MimeType.
    /// 
    /// Logic copied from TrafficCourts.Common.Features.FilePersistence.FilePersistenceService#GetMimeTypeAsync(Stream data) 
    /// since that method is protected and not accessible.
    /// </summary>
    /// <param name="data">A stream to scan for a MimeType</param>
    /// <returns></returns>
    public static async Task<MimeType> GetMimeTypeAsync(this Stream data)
    {
        ArgumentNullException.ThrowIfNull(data);

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

}
