using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public class OracleDataApiService : IOracleDataApiService
    {
        private readonly ILogger<OracleDataApiService> _logger;
        private readonly OracleDataApiConfiguration _oracleDataApiConfiguration;

        public OracleDataApiService(ILogger<OracleDataApiService> logger, IOptions<OracleDataApiConfiguration> oracleDataApiConfiguration)
        {
            _logger = logger;
            _oracleDataApiConfiguration = oracleDataApiConfiguration.Value;
        }

        public async Task<int> CreateDisputeAsync(Dispute disputeToSubmit)
        {
            // Formatting all dispute class properties to camel case since oracle data api
            // accepts camel case only after serialization
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            using (var httpClient = new HttpClient())
            {
                var oracleDataApiBaseUri = $"http://{_oracleDataApiConfiguration.Host}:{_oracleDataApiConfiguration.Port}";
                httpClient.BaseAddress = new Uri(oracleDataApiBaseUri);
                using (var response = await httpClient.PostAsync("dispute", disputeToSubmit, formatter))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsAsync<int>();
                        return apiResponse;
                    }

                    return -1; 
                }
            }
        }
    }
}
