using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public class OracleInterfaceService : IOracleInterfaceService
    {
        private readonly ILogger<OracleInterfaceService> _logger;
        private readonly OracleInterfaceApiConfiguration _oracleInterfaceApiConfiguration;

        public OracleInterfaceService(ILogger<OracleInterfaceService> logger, IOptions<OracleInterfaceApiConfiguration> oracleInterfaceApiConfiguration)
        {
            _logger = logger;
            _oracleInterfaceApiConfiguration = oracleInterfaceApiConfiguration.Value;
        }

        public async Task<int> CreateDisputeAsync(Dispute disputeToSubmit)
        {
            // Formatting all dispute class properties to camel case since oracle data interface api
            // accepts camel case only after serialization
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            using (var httpClient = new HttpClient())
            {
                var orafaceBaseUri = $"http://{_oracleInterfaceApiConfiguration.Host}:{_oracleInterfaceApiConfiguration.Port}";
                httpClient.BaseAddress = new Uri(orafaceBaseUri);
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
