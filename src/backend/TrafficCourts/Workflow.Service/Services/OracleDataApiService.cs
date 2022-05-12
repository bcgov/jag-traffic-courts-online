using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using TrafficCourts.Messaging.MessageContracts;
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

        public async Task<Guid> CreateDisputeAsync(NoticeOfDispute disputeToSubmit)
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
                var oracleDataApiBaseUri = $"{_oracleDataApiConfiguration.BaseUrl}api/v1.0/";
                httpClient.BaseAddress = new Uri(oracleDataApiBaseUri);
                using (var response = await httpClient.PostAsync("dispute", disputeToSubmit, formatter))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsAsync<Guid>();
                        return apiResponse;
                    }

                    return Guid.Empty; 
                }
            }
        }
    }
}
