using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public class OracleInterfaceService : IOracleInterfaceService
    {
        private readonly ILogger<OracleInterfaceService> _logger;

        public OracleInterfaceService(ILogger<OracleInterfaceService> logger)
        {
            _logger = logger;
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
                httpClient.BaseAddress = new Uri(@"http://localhost:5010/");
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
