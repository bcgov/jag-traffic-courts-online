using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using ApiException = TrafficCourts.Common.OpenAPIs.VirusScan.V1.ApiException;

namespace TrafficCourts.Workflow.Service.Services
{
    public class VirusScanService : IVirusScanService
    {
        private readonly ILogger<VirusScanService> _logger;
        private readonly IVirusScanClient _virusScanClient;

        public VirusScanService(ILogger<VirusScanService> logger, IVirusScanClient virusScanClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _virusScanClient = virusScanClient ?? throw new ArgumentNullException(nameof(virusScanClient));
        }

        public async Task<ScanResponse> ScanDocumentAsync(Stream file, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Virus scanning the file through ClamAV");
                return await _virusScanClient.ScanFileAsync(file, cancellationToken);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Could not scan the file for viruses");
                throw ex;
            }
        }
    }
}
