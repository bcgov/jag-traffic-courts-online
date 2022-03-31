using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public class SubmitDisputeToArcService : ISubmitDisputeToArcService
    {
        private readonly ILogger<SubmitDisputeToArcService> _logger;

        public SubmitDisputeToArcService(ILogger<SubmitDisputeToArcService> logger)
        {
            _logger = logger;
        }

        public async Task SubmitDisputeToArcAsync(TcoDisputeTicket approvedDispute)
        {
            // Formatting all TcoDisputeTicket class properties to camel case since ARC api accepts kebab case only after serialization
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver { NamingStrategy = new KebabCaseNamingStrategy() }
            };

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"https://localhost:7082/");

            using var response = await httpClient.PostAsync("api/TcoDisputeTicket", approvedDispute, formatter);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                if (apiResponse != null)
                {
                    _logger.LogInformation("An ARC file has been created successfully with the following data: " + apiResponse);
                }
            }
        }
    }
}
