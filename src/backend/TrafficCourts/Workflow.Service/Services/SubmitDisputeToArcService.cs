using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public class SubmitDisputeToArcService : ISubmitDisputeToArcService
    {
        private readonly ILogger<SubmitDisputeToArcService> _logger;
        private readonly ArcApiConfiguration _arcApiConfiguration;

        public SubmitDisputeToArcService(ILogger<SubmitDisputeToArcService> logger, IOptions<ArcApiConfiguration> arcApiConfiguration)
        {
            _logger = logger;
            _arcApiConfiguration = arcApiConfiguration.Value;
        }

        public async Task SubmitDisputeToArcAsync(TcoDisputeTicket approvedDispute)
        {
            // Formatting all TcoDisputeTicket class properties to camel case since ARC api accepts kebab case only after serialization
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
            };

            using var httpClient = new HttpClient();
            var arcBaseUri = $"http://{_arcApiConfiguration.Host}:{_arcApiConfiguration.Port}";
            httpClient.BaseAddress = new Uri(arcBaseUri);

            using var response = await httpClient.PostAsync("api/TcoDisputeTicket", approvedDispute, formatter);
            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (apiResponse != null)
                {
                    _logger.LogInformation("An ARC file has been created successfully with the following data: {ArcApiResponse}", apiResponse);
                }
            }
            else
            {
                var status = (int)response.StatusCode;
                _logger.LogError("The request for creating ARC file has failed with the following response: {ArcApiResponse} ", apiResponse);
                throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, apiResponse);
            }
        }
    }

    [Serializable]
    class ApiException : Exception
    {

        public int StatusCode { get; private set; }

        public string? Response { get; private set; }

        public ApiException()
        {
        }

        public ApiException(string? message, int statusCode, string? response) : base(message)
        {
            StatusCode = statusCode;
            Response = response;
        }

        public ApiException(string message, int statusCode, string? response, Exception? innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}
