using System.Buffers;
using System.Text;

namespace System.IO;

public static class StreamExtensions
{
    /// <summary>
    /// Scans the first 1K of a data stream looking for the MimeType.
    /// 
    /// Logic copied from TrafficCourts.Common.Features.FilePersistence.FilePersistenceService#GetMimeTypeAsync(Stream data) 
    /// since that method is protected and not accessible.
    /// </summary>
    /// <param name="stream">A stream to scan for a MimeType</param>
    /// <returns></returns>
    public static FileMimeType? GetMimeType(this MemoryStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var pool = ArrayPool<byte>.Shared;

        // get a buffer to read the first 8 bytes of the file stream
        // none of our target matches are longer than this
        byte[] buffer = pool.Rent(8);

        var position = stream.Position; // save the current buffer position

        try
        {
            stream.Seek(0L, SeekOrigin.Begin); // ensure we are at the begining of the stream
            int count = stream.Read(buffer, 0, buffer.Length);
            // create span containing the data read
            Span<byte> span = buffer.AsSpan<byte>(0, count);

            foreach (var mimeTypeMatch in _mimeTypeMatches)
            {
                if (span.StartsWith(mimeTypeMatch.Buffer.Span))
                {
                    return new FileMimeType(MimeType: mimeTypeMatch.MimeType, Extension: mimeTypeMatch.Extension);
                }
            }

            return null;
        }
        finally
        {
            stream.Seek(position, SeekOrigin.Begin); // reset the memory stream to the beginning
            pool.Return(buffer, clearArray: true);
        }
    }

    /// <summary>
    /// Private record to hold expected file prefix for each mine type.
    /// </summary>
    /// <param name="MimeType">The mime type of the file</param>
    /// <param name="Extension">The normalized file extension.</param>
    /// <param name="Buffer"></param>
    /// <remarks>
    /// Based on https://github.com/GetoXs/MimeDetect/blob/master/src/Winista.MimeDetect/mime-types.xml
    /// </remarks>
    private record MimeTypeMatch(string MimeType, string Extension, Memory<byte> Buffer) : FileMimeType(MimeType, Extension);

    private static readonly MimeTypeMatch[] _mimeTypeMatches = new MimeTypeMatch[]
    {
        // sorted based on expected most occurences
        new MimeTypeMatch("image/jpeg", "jpg", new Memory<byte>(new byte[] { 0xff, 0xd8, 0xff })),
        new MimeTypeMatch("application/pdf", "pdf", new Memory<byte>(Encoding.ASCII.GetBytes("%PDF-"))),
        new MimeTypeMatch("image/png", "png", new Memory<byte>(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a }))
    };
}
