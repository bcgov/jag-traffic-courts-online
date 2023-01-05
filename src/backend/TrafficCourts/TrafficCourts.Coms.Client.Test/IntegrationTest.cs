using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace TrafficCourts.Coms.Client.Test
{
    [ExcludeFromCodeCoverage(Justification = "Integration Test")]
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

            _client = new ObjectManagementClient(httpClient);
            _client.BaseUrl = baseUrl;
            _output = output;
        }

        [IntegrationTestFact]
        public async Task create_and_delete()
        {
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
            await _client.DeleteObjectAsync(id, null, _cancellationToken);
        }

        [IntegrationTestFact]
        public async Task create_read_and_delete()
        {
            Guid expected = Guid.NewGuid();

            var file = new FileParameter(new MemoryStream(expected.ToByteArray()), "integration.test");
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

                await _client.DeleteObjectAsync(item.Id, null, _cancellationToken);
            }
        }

        public static IEnumerable<object[]> ObjectsToDelete
        {
            get
            {
                yield return new object[] { new Guid("300f1a55-66f3-40dc-a1f6-326b05ccbbd2") };
            }
        }
    }
}
