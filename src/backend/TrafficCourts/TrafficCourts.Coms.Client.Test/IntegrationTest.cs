using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace TrafficCourts.Coms.Client.Test
{
    public class IntegrationTest
    {
        // This test requires user secrets in this format. Also, you need to define INTEGRATION_TEST 
        // in your build. Go to project properties > Build > Conditional Compilation Symbols
        //
        // {
        //   "BaseUrl": "https://coms-host/api/v1",
        //   "Username": "coms-username",
        //   "Password": "coms-password"
        // }
        //
        private readonly ITestOutputHelper _output;
        private ObjectManagementClient _client;
        private CancellationToken _cancellationToken = CancellationToken.None;
        private ObjectManagementService _service;

        public IntegrationTest(ITestOutputHelper output)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<IntegrationTest>()
                .Build();

            var baseUrl = configuration.GetValue<string>("BaseUrl") ?? string.Empty;
            var username = configuration.GetValue<string>("Username") ?? string.Empty;
            var password = configuration.GetValue<string>("Password") ?? string.Empty;

            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(username, password);

            // create client 
            _client = new ObjectManagementClient(httpClient);
            _client.BaseUrl = baseUrl;
            _client.ReadResponseAsString = true; // make it easier to debug
            _output = output;

            // create service 
            var factory = new MemoryStreamFactory(() => new MemoryStream());
            _service = new ObjectManagementService(_client, factory, Mock.Of<ILogger<ObjectManagementService>>());
        }

        #region ObjectManagementClient
        [IntegrationTestTheory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task create_and_delete(bool readResponseAsString)
        {
            _client.ReadResponseAsString = readResponseAsString;

            Guid expected = Guid.NewGuid();

            var file = new FileParameter(new MemoryStream(expected.ToByteArray()), "integration.test");

            ICollection<Anonymous> created = await _client.CreateObjectsAsync(null, null, file, _cancellationToken);

            _output.WriteLine("Created {0} items", created.Count);

            foreach (var item in created)
            {
                _output.WriteLine("Delete item {0}", item.Id);
                await _client.DeleteObjectAsync(item.Id, null, _cancellationToken);
                _output.WriteLine("Deleted item {0}", item.Id);
            }
        }

        [IntegrationTestTheory]
        [MemberData(nameof(ObjectsToDelete))]
        public async Task delete_objects(Guid id)
        {
            // this should throw ApiException<ResponseError> with Detail = "NotFoundError"
            // if the source file does not exist
            await _client.DeleteObjectAsync(id, null, _cancellationToken);
        }

        [IntegrationTestTheory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task create_read_and_delete(bool readResponseAsString)
        {
            _client.ReadResponseAsString = readResponseAsString;           

            Guid expected = Guid.NewGuid();

            var file = new FileParameter(new MemoryStream(expected.ToByteArray()), "integration.test");
            _client.ReadResponseAsString = true; // make it easier troubleshoot reading the responses
            ICollection<Anonymous> created = await _client.CreateObjectsAsync(null, null, file, CancellationToken.None);

            _output.WriteLine("Created: ");
            foreach (var item in created)
            {
                _output.WriteLine("  {0}", item.Id);
                var response = await _client.ReadObjectAsync(item.Id, DownloadMode.Proxy, null, null , _cancellationToken);

                MemoryStream stream = new MemoryStream();
                await response.Stream.CopyToAsync(stream);
                var actual = new Guid(stream.ToArray());

                Assert.Equal(expected, actual);

                //await _client.DeleteObjectAsync(item.Id, null, _cancellationToken);
            }
        }

        public static IEnumerable<object[]> ObjectsToDelete
        {
            get
            {
                yield return new object[] { new Guid("a56bd633-c826-4043-a29c-a40f1529f57b") };
            }
        }
        #endregion

        #region ObjectManagementService
        [IntegrationTestFact]
        public async Task create_file_from_service()
        {
            Guid expectedData = Guid.NewGuid();

            string filename = $"test-{expectedData.ToString("n")[0..6]}.bin";
            File expectedFile = new File(new MemoryStream(expectedData.ToByteArray()), filename, "a/b");

            for (int i = 0; i < 5; i++)
            {
                expectedFile.Metadata.Add(Guid.NewGuid().ToString("n")[0..6], Guid.NewGuid().ToString("n"));
                expectedFile.Tags.Add(Guid.NewGuid().ToString("n")[0..6], Guid.NewGuid().ToString("n"));
            }

            // expect the file id to be null before creation
            Assert.Null(expectedFile.Id);
            var id = await _service.CreateFileAsync(expectedFile, _cancellationToken);

            // expect the file id to set after creation
            Assert.Equal(id, expectedFile.Id);

            using var actualFile = await _service.GetFileAsync(id, false, _cancellationToken);

            // search for the file by the first metadata property
            FileSearchParameters parameters = new FileSearchParameters();
            string key = expectedFile.Metadata.Keys.First();
            parameters.Metadata.Add(key, expectedFile.Metadata[key]);

            var fileSearchResults = await _service.FileSearchAsync(parameters, _cancellationToken);

            FileSearchResult actualFound = Assert.Single(fileSearchResults);
            Assert.Equal(id, actualFound.Id);
            Assert.Equal(filename, actualFound.FileName);
            Assert.Equal(expectedFile.Metadata.Count, actualFound.Metadata.Count);

            await _service.DeleteFileAsync(id, _cancellationToken);

            //Assert.Equal(expectedData.ToByteArray(), actualFile.Data);
        }


        [IntegrationTestTheory]
        [MemberData(nameof(ObjectsToDelete))]
        public async Task delete_objects_from_service(Guid id)
        {
            await _service.DeleteFileAsync(id, _cancellationToken);
        }



        #endregion
    }
}
