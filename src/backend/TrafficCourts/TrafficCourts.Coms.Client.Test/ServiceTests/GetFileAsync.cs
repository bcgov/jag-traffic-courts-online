using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Newtonsoft.Json.Linq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class GetFileAsync : ObjectManagementServiceTest
{
    [Fact]
    public async Task should_copy_data_when_file_is_found()
    {
        Guid id = Guid.NewGuid();

        // create some sample data data
        Dictionary<string, IEnumerable<string>> headers = new()
        {
            { "x-amz-meta-id", new string[] { id.ToString("d") } },
            { "x-amz-meta-name", new string[] { "filename.png" } }
        };

        List<MetadataItem> metadataItems = new List<MetadataItem>
        {
            new MetadataItem { Key = "type", Value = "picture" }
        };

        var metadata = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("type", "picture")
        };

        var stream = GetRandomStream();
        SetupReadObjectReturn(new FileResponse(200, headers, stream, null, null));
        SetupGetObjectMetadataAsync(new Anonymous2[] { new Anonymous2 { ObjectId = id, Metadata = new List<DBMetadataKeyValue> { new DBMetadataKeyValue { Key = "type", Value = "picture" } } } });
        SetupGetObjectTagsAsync(new Anonymous3[] { new Anonymous3 { ObjectId = id } });

        CancellationTokenSource cts = new CancellationTokenSource();

        ObjectManagementService sut = GetService();

        // act
        File actualFile = await sut.GetFileAsync(id, cts.Token);

        // assert
        // should have read the object to get the expected found object
        _mockClient.Verify(_ => _.ReadObjectAsync(
            It.Is<Guid>((objId) => objId == id),
            It.Is<DownloadMode?>((download) => download == DownloadMode.Proxy),
            It.Is<int?>((expiresIn) => expiresIn == null),
            It.Is<string?>((versionId) => versionId == null),
            It.Is<CancellationToken>((cancellationToken) => cancellationToken == cts.Token)
        ));

        Assert.NotNull(actualFile);
        Assert.NotNull(actualFile.Data);
        Assert.NotEqual(stream, actualFile.Data); // should NOT return the same stream
        Assert.Equal("filename.png", actualFile.FileName);

        // should be only 1 meta data field
        Assert.Single(actualFile.Metadata);
        Assert.Equal("picture", actualFile.Metadata["type"]);

        // expect the file position to be at the beginning of the stream
        Assert.Equal(0L, actualFile.Data.Position);

        var expected = Assert.IsAssignableFrom<MemoryStream>(stream).ToArray();
        var actual = Assert.IsAssignableFrom<MemoryStream>(actualFile.Data).ToArray();

        // ensure all the data matches
        Assert.Equal(expected.Length, actual.Length); // data size should be same
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }
}
