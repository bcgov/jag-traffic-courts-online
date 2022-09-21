using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace TrafficCourts.Common.Test;

[ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
public class GetMimeTypeTest
{
    [Theory(Skip = "Supply your own files")]
    [InlineData(@"D:\Downloads\filename.png", "png", "image/png", 0)]
    [InlineData(@"D:\Downloads\filename.png", "png", "image/png", 10)]
    [InlineData(@"D:\Downloads\filename.pdf", "pdf", "application/pdf", 0)]
    [InlineData(@"D:\Downloads\filename.pdf", "pdf", "application/pdf", 10)]
    [InlineData(@"D:\Downloads\filename.jpg", "jpg", "image/jpeg", 0)]
    [InlineData(@"D:\Downloads\filename.jpg", "jpg", "image/jpeg", 10)]
    public void can_get_mime_type_for_files(string path, string extension, string mimeType, long streamPosition)
    {
        byte[] buffer = File.ReadAllBytes(path);

        MemoryStream stream = new MemoryStream(buffer);
        stream.Seek(streamPosition, SeekOrigin.Begin);

        FileMimeType? fileMimeType = stream.GetMimeType();

        Assert.NotNull(fileMimeType);
        Assert.Equal(extension, fileMimeType!.Extension);
        Assert.Equal(mimeType, fileMimeType.MimeType);
        Assert.Equal(streamPosition, stream.Position); // ensure file position is reset
    }

    [Fact]
    public void random_data_does_not_have_mime_type()
    {
        // 1 out of 16777216 times could cause this test to fail (jpg only matches on first 3 bytes)
        MemoryStream stream = new(RandomNumberGenerator.GetBytes(256));

        var fileMimeType = stream.GetMimeType();
        Assert.Null(fileMimeType);
    }

    [Fact]
    public void empty_data_does_not_have_mime_type()
    {
        MemoryStream stream = new();
        var fileMimeType = stream.GetMimeType();
        Assert.Null(fileMimeType);
    }

    [Fact]
    public void null_stream_throws_ArgumentNullException()
    {
        MemoryStream stream = null!;
        Assert.Throws<ArgumentNullException>(() => stream.GetMimeType());
    }
}
