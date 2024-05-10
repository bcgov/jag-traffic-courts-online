using Xunit.Abstractions;

namespace TrafficCourts.Coms.Client.Test
{
    public class ComsIntegrationTest
    {
        private readonly ITestOutputHelper _output;

        public ComsIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        //[Fact]
        public async Task test_coms_operations()
        {
            ComsContainers containers = new();

            await containers.BuildAndStartAsync(CancellationToken.None);
            _output.WriteLine("started");

            byte[] buffer = new byte[4 * 1024];
            var stream = new MemoryStream(buffer);

            Dictionary<string, string> meta = [];
            Dictionary<string, string> tags = [];
            tags.Add("a", "1");

            var client = containers.GetObjectManagementClient();
            var objects = await client.CreateObjectsAsync(meta, tags, null, new FileParameter(stream, "unittest.txt", "application/json"));

            var objectId = objects[0].Id;

            FileResponse file = await client.ReadObjectAsync(objectId, DownloadMode.Proxy, null, null, CancellationToken.None);

            // test fetch and add tags
            IList<Anonymous3> fetchTagsResponse = await client.FetchTagsAsync([objectId], null, CancellationToken.None);

            var objectTags = Assert.Single(fetchTagsResponse.Where(_ => _.ObjectId == objectId));
            var objectTagValue = Assert.Single(objectTags.Tagset);
            Assert.Equal("a", objectTagValue.Key);
            Assert.Equal("1", objectTagValue.Value);

            tags.Add("b", "2");
            tags.Remove("a");

            await client.AddTaggingAsync(objectId, tags, null);
            fetchTagsResponse = await client.FetchTagsAsync([objectId]);

            objectTags = Assert.Single(fetchTagsResponse.Where(_ => _.ObjectId == objectId));
            Assert.Equal(2, objectTags.Tagset.Count);

            // 
            IList<Anonymous2> fetchMetadataResponse = await client.FetchMetadataAsync([objectId], null, CancellationToken.None);
            var objectMetadata = Assert.Single(fetchMetadataResponse.Where(_ => _.ObjectId == objectId));

            meta = ToDictionary(objectMetadata);
            //meta["coms-name"] = "something-new.txt";
            meta.Add("size", "4096");

            try
            {
                await client.AddMetadataAsync(meta, objectId, null, CancellationToken.None);
            }
            catch (ApiException)
            {

            }

            fetchMetadataResponse = await client.FetchMetadataAsync([objectId], null, CancellationToken.None);
            objectMetadata = Assert.Single(fetchMetadataResponse.Where(_ => _.ObjectId == objectId));
        }

        private Dictionary<string, string> ToDictionary(Anonymous2 objectMetadata)
        {
            return objectMetadata.Metadata.ToDictionary(_ => _.Key, _ => _.Value);
        }

        private Dictionary<string, string> ToDictionary(Anonymous3 objectTags)
        {
            return objectTags.Tagset.ToDictionary(_ => _.Key, _ => _.Value);
        }
    }
}
