using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace TrafficCourts.Coms.Client.Test
{
    public class ComsWithVersioningIntegrationTest : ComsIntegrationTest
    {
        public ComsWithVersioningIntegrationTest(ITestOutputHelper output) : base(true, output)
        {   
        }
    }

    public class ComsWithoutVersioningIntegrationTest : ComsIntegrationTest
    {
        public ComsWithoutVersioningIntegrationTest(ITestOutputHelper output) : base(false, output)
        {
        }
    }

    public abstract class ComsIntegrationTest : IAsyncLifetime
    {
        private readonly bool _versioningEnabled;
        private readonly ITestOutputHelper _output;

        private ComsContainers _containers;

        protected ComsIntegrationTest(bool versioningEnabled, ITestOutputHelper output)
        {
            _output = output;
            _versioningEnabled = versioningEnabled;
        }

        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync()
        {
            await _containers.DisposeAsync().AsTask();
        }

        [IntegrationTestTheory]
        [InlineData("0.4.2")]
        [InlineData("0.5.0")]
        public async Task client_can_delete_files(string version)
        {
            _containers = new(version);
            await _containers.BuildAndStartAsync(_versioningEnabled, CancellationToken.None);

            var client = _containers.GetObjectManagementClient();

            // create a file
            var id = await CreateFileAsync(client);

            // ensure the file can be retrieved
            var file = await client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, s3VersionId: null, CancellationToken.None);
            Assert.NotNull(file);

            // delete the file
            await client.DeleteObjectAsync(id, null);

            var e = await Assert.ThrowsAsync<ApiException<ResponseNotFound>>(() => client.ReadObjectAsync(id, DownloadMode.Proxy, expiresIn: null, s3VersionId: null, CancellationToken.None));
            Assert.Equal(404, e.StatusCode);
        }

        [IntegrationTestTheory]
        [InlineData("0.4.2")]
        [InlineData("0.5.0")]
        public async Task service_can_delete_files(string version)
        {
            _containers = new(version);
            await _containers.BuildAndStartAsync(_versioningEnabled, CancellationToken.None);

            var client = _containers.GetObjectManagementClient();
            var service = _containers.GetObjectManagementService();

            // create a file
            var id = await CreateFileAsync(client);

            // ensure the file can be retrieved
            File file = await service.GetFileAsync(id, CancellationToken.None);
            Assert.NotNull(file);
            Assert.Equal(id, file.Id);

            // delete the file
            await service.DeleteFileAsync(id, CancellationToken.None);

            // ensure the file cannot be retrieved
            var actual = Assert.ThrowsAsync<FileNotFoundException>(() => service.GetFileAsync(id, CancellationToken.None));
        }

        [IntegrationTestTheory]
        [InlineData("0.4.2")]
        [InlineData("0.5.0")]
        public async Task client_after_delete_file_search_files_does_not_find_file(string version)
        {
            _containers = new(version);
            await _containers.BuildAndStartAsync(_versioningEnabled, CancellationToken.None);

            var client = _containers.GetObjectManagementClient();

            // create a file
            var tags = new Dictionary<string, string>
            {
                { "foobar", "delete_me" }
            };

            var id = await CreateFileAsync(client, null, tags);

            List<DBObject> searchResult = await client.SearchObjectsAsync(null, null, null, null, null, null, null, null, null, null, tags, CancellationToken.None);

            Assert.Single(searchResult);

            // delete the file
            ResponseObjectDeleted deleteResponse = await client.DeleteObjectAsync(searchResult[0].Id, null);

            searchResult = await client.SearchObjectsAsync(null, null, null, null, null, null, true, null, null, null, tags, CancellationToken.None);

            Assert.Empty(searchResult);
        }

        [IntegrationTestTheory]
        [InlineData("0.4.2")]
        [InlineData("0.5.0")]
        public async Task service_after_delete_file_search_files_does_not_find_file(string version)
        {
            _containers = new(version);
            await _containers.BuildAndStartAsync(_versioningEnabled, CancellationToken.None);

            var client = _containers.GetObjectManagementClient();
            var service = _containers.GetObjectManagementService();

            // create a file
            var tags = new Dictionary<string, string>
            {
                { "foobar", "delete_me" }
            };

            var id = await CreateFileAsync(client, null, tags);

            FileSearchParameters parameters = new FileSearchParameters(null, null, tags);
            IList<FileSearchResult> searchResult = await service.FileSearchAsync(parameters, CancellationToken.None);

            Assert.Single(searchResult);

            // delete the file
            await service.DeleteFileAsync(searchResult[0].Id, CancellationToken.None);

            searchResult = await service.FileSearchAsync(parameters, CancellationToken.None);

            Assert.Empty(searchResult);
        }

        [IntegrationTestTheory]
        [InlineData("0.4.2")]
        [InlineData("0.5.0")]
        public async Task test_coms_operations(string version)
        {
            _containers = new(version);
            await _containers.BuildAndStartAsync(_versioningEnabled, CancellationToken.None);

            byte[] buffer = new byte[4 * 1024];
            var stream = new MemoryStream(buffer);

            Dictionary<string, string> meta = [];
            Dictionary<string, string> tags = [];
            tags.Add("a", "1");

            var client = _containers.GetObjectManagementClient();
            var objects = await client.CreateObjectsAsync(meta, tags, null, new FileParameter(stream, "unittest.txt", "application/json"));

            var objectId = objects[0].Id;

            FileResponse file = await client.ReadObjectAsync(objectId, DownloadMode.Proxy, null, null, CancellationToken.None);

            // test fetch and add tags
            IList<Anonymous3> fetchTagsResponse = await client.FetchTagsAsync([objectId], null, CancellationToken.None);

            var objectTags = Assert.Single(fetchTagsResponse, _ => _.ObjectId == objectId);

            var comsVersion = _containers.Coms.ContainerVersion();

            if (comsVersion.Minor == 4)
            {
                var objectTagValue = Assert.Single(objectTags.Tagset);
                Assert.Equal("a", objectTagValue.Key);
                Assert.Equal("1", objectTagValue.Value);
            }
            else //if (comsVersion.Minor == 5)
            {
                // 0.5.0 adds coms-id tag
                var objectTagValue = Assert.Single(objectTags.Tagset, _ => _.Key == "a");
                Assert.Equal("a", objectTagValue.Key);
                Assert.Equal("1", objectTagValue.Value);

                objectTagValue = Assert.Single(objectTags.Tagset, _ => _.Key == "coms-id");
                Assert.Equal("coms-id", objectTagValue.Key);
                Assert.Equal(objectId.ToString(), objectTagValue.Value);
            }

            tags.Add("b", "2");
            tags.Remove("a");

            await client.AddTaggingAsync(objectId, tags, null);
            fetchTagsResponse = await client.FetchTagsAsync([objectId]);

            if (comsVersion.Minor == 4)
            {
                objectTags = Assert.Single(fetchTagsResponse, _ => _.ObjectId == objectId);
                Assert.Equal(2, objectTags.Tagset.Count);
            }
            else //if (comsVersion.Minor == 5)
            {
                objectTags = Assert.Single(fetchTagsResponse, _ => _.ObjectId == objectId);
                Assert.Equal(3, objectTags.Tagset.Count);
            }

            // 
            IList<Anonymous2> fetchMetadataResponse = await client.FetchMetadataAsync([objectId], null, CancellationToken.None);
            var objectMetadata = Assert.Single(fetchMetadataResponse, _ => _.ObjectId == objectId);

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
            objectMetadata = Assert.Single(fetchMetadataResponse, _ => _.ObjectId == objectId);
        }

        private Dictionary<string, string> ToDictionary(Anonymous2 objectMetadata)
        {
            return objectMetadata.Metadata.ToDictionary(_ => _.Key, _ => _.Value);
        }

        private Dictionary<string, string> ToDictionary(Anonymous3 objectTags)
        {
            return objectTags.Tagset.ToDictionary(_ => _.Key, _ => _.Value);
        }

        private async Task<Guid> CreateFileAsync(ObjectManagementClient client, Dictionary<string, string>? meta = null, Dictionary<string, string>? tags = null)
        {
            byte[] buffer = new byte[4 * 1024];
            var stream = new MemoryStream(buffer);

            meta ??= [];
            tags ??= [];

            var objects = await client.CreateObjectsAsync(meta, tags, null, new FileParameter(stream, "unittest.txt", "application/json"));

            Guid objectId = objects[0].Id;
            return objectId;
        }
    }
}
