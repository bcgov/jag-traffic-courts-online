using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Configuration;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace TrafficCourts.Workflow.Service.Services;

/// <summary>
/// Summary description for FileHistoryService
/// </summary>
public class FileHistoryService : IFileHistoryService
{
    private readonly OracleDataApiConfiguration _oracleDataApiConfiguration;
    private readonly ILogger<FileHistoryService> _logger;
    private readonly IBus _bus;
    private readonly IFilePersistenceService _filePersistenceService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public FileHistoryService(
        OracleDataApiConfiguration oracleDataApiConfiguration,
        IBus bus,
        IFilePersistenceService filePersistenceService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileHistoryService> logger)
    {
        _oracleDataApiConfiguration = oracleDataApiConfiguration ?? throw new ArgumentNullException(nameof(oracleDataApiConfiguration));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a new initialized instance of the OracleDataApi_v1_0Client
    /// </summary>
    /// <returns></returns>
    private OracleDataApiClient GetOracleDataApi()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(_oracleDataApiConfiguration.BaseUrl) };
        OracleDataApiClient client = new(httpClient);

        var user = _httpContextAccessor.HttpContext?.User;

        var username = user?.Claims?.FirstOrDefault(_ => _.Type == "preferred_username")?.Value;
        var fullName = user?.Claims?.FirstOrDefault(_ => _.Type == ClaimTypes.Name)?.Value;

        if (username is not null && !string.IsNullOrWhiteSpace(username))
        {
            // we expect the username to be of the form: someone@domain
            int index = username.IndexOf("@");
            if (index > 0)
            {
                username = username[..index];
            }

            HttpRequestHeaders requestHeaders = httpClient.DefaultRequestHeaders;
            requestHeaders.Add("x-username", username);

            if (fullName is not null && !string.IsNullOrWhiteSpace(fullName))
            {
                requestHeaders.Add("x-fullName", fullName);
            }
        }
        else
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                // this is being executed outside of an web request
                _logger.LogError("Cannot set x-username header, no HttpContext is available, the request not executing part of a HTTP web api request");
            }
            else
            {                
                using var scope = _logger.BeginScope(new Dictionary<string, object> {
                    ["IsAuthenticated"] = _httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated ?? false,
                    ["AuthenticationType"] = _httpContextAccessor.HttpContext.User.Identity?.AuthenticationType ?? String.Empty
                });

                _logger.LogError("Could not find preferred_username claim on current user");
            }
        }

        return client;
    }

    public async Task<long> SaveFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().InsertFileHistoryAsync(fileHistory.TicketNumber, fileHistory, cancellationToken);
    }
}
