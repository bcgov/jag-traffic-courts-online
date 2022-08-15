using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TrafficCourts.Arc.Dispute.Client.Test
{
    public class AddArcDisputeClientTest
    {
        private ServiceCollection _services = new ServiceCollection();
        private Fixture _fixture = new Fixture();

        [Fact]
        public void AddArcDisputeClient_configures_client_with_correct_base_address()
        {
            // arrange
            Configuration clientConfiguration = GetClientConfiguration();

            IConfiguration configuration = BuildConfiguration("Arc", clientConfiguration);

            // act
            _services.AddArcDisputeClient(configuration, "Arc");

            // assert
            var serviceProvider = _services.BuildServiceProvider();

            IArcDisputeClient actual = serviceProvider.GetRequiredService<IArcDisputeClient>();

            // make sure the client has the correct base address
            HttpClient httpClient = GetHttpClientFrom(actual);

            Assert.Equal(clientConfiguration.Uri, httpClient.BaseAddress);
        }

        private IConfiguration BuildConfiguration(string section, Configuration configuration)
        {
            var items = new Dictionary<string, string>
            {
                { $"{section}:{nameof(Configuration.Scheme)}", configuration.Scheme },
                { $"{section}:{nameof(Configuration.Host)}", configuration.Host },
                { $"{section}:{nameof(Configuration.Port)}", configuration.Port.ToString() }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(items)
                .Build();
        }

        private Configuration GetClientConfiguration()
        {
            Configuration configuration = new Configuration();
            configuration.Host = _fixture.Create<string>();
            configuration.Port = Random.Shared.Next(ushort.MinValue, ushort.MaxValue);
            return configuration;
        }

        private HttpClient GetHttpClientFrom(IArcDisputeClient client)
        {
            FieldInfo? field = typeof(ArcDisputeClient)
                .GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(field);

            HttpClient httpClient = field.GetValue(client) as HttpClient;
            return httpClient;
        }
    }
}