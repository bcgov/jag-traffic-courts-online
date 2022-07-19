using Microsoft.Extensions.Options;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Configuration;

namespace TrafficCourts.Workflow.Service.Services;

public class OracleDataApiService : IOracleDataApiService
{
    private readonly ILogger<OracleDataApiService> _logger;
    private readonly OracleDataApiConfiguration _oracleDataApiConfiguration;

    public OracleDataApiService(ILogger<OracleDataApiService> logger, IOptions<OracleDataApiConfiguration> oracleDataApiConfiguration)
    {
        _logger = logger;
        _oracleDataApiConfiguration = oracleDataApiConfiguration.Value;
    }

    public async Task<long> CreateDisputeAsync(Dispute dispute)
    {
        // stub out the ViolationTicket if the submitted Dispute has associated OCR scan results.
        if (!string.IsNullOrEmpty(dispute.OcrViolationTicket))
        {
            dispute.ViolationTicket = new();
            
            // TODO: initialize ViolationTicket with data from OCR 
        }

        return await GetOracleDataApi().SaveDisputeAsync(dispute);
    }

    /// <summary>
    /// Returns a new inialized instance of the OracleDataApi_v1_0Client
    /// </summary>
    /// <returns></returns>
    private OracleDataApiClient GetOracleDataApi()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(_oracleDataApiConfiguration.BaseUrl) };
        OracleDataApiClient client = new(httpClient);
        return client;
    }
}
