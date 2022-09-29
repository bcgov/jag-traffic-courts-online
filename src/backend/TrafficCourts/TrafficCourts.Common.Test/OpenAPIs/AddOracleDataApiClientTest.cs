using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI;
using Xunit;

namespace TrafficCourts.Common.Test.OpenAPIs
{
    public class AddOracleDataApiClientTest
    {
        [Fact]
        public void OracleDataApiClient_configures_client_with_correct_base_address()
        {
            // arrange
            ServiceCollection services = new ServiceCollection();
            OracleDataApiConfiguration clientConfiguration = new OracleDataApiConfiguration
            {
                BaseUrl = $"https://localhost-{Guid.NewGuid():n}"
            };

            IConfiguration configuration = BuildConfiguration(OracleDataApiConfiguration.Section, clientConfiguration);

            // act
            services.AddOracleDataApiClient(configuration);

            // assert
            var serviceProvider = services.BuildServiceProvider();

            IOracleDataApiClient actual = serviceProvider.GetRequiredService<IOracleDataApiClient>();

            // make sure the client has the correct base address
            HttpClient? httpClient = GetHttpClientFrom(actual);

            Assert.NotNull(httpClient);
            Assert.Equal(new Uri(clientConfiguration.BaseUrl), httpClient!.BaseAddress);
        }

        private HttpClient? GetHttpClientFrom(IOracleDataApiClient client)
        {
            FieldInfo? field = typeof(OracleDataApiClient)
                .GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(field);

            HttpClient? httpClient = field!.GetValue(client) as HttpClient;
            return httpClient;
        }

        private IConfiguration BuildConfiguration(string section, OracleDataApiConfiguration configuration)
        {
            var items = new Dictionary<string, string>
            {
                { $"{section}:{nameof(OracleDataApiConfiguration.BaseUrl)}", configuration.BaseUrl }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(items)
                .Build();
        }
    }
}
