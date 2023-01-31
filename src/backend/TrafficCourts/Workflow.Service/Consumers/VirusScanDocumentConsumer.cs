using MassTransit;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.VirusScan.V1.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Consumer for VirusScanDocument message.
    /// </summary>
    public class VirusScanDocumentConsumer : IConsumer<VirusScanDocument>
    {
        private readonly IVirusScanClient _virusScan;
        private readonly IComsService _comsService;
        private readonly ILogger<VirusScanDocumentConsumer> _logger;

        public VirusScanDocumentConsumer(IVirusScanClient virusScan, IComsService comsService, ILogger<VirusScanDocumentConsumer> logger)
        {
            _virusScan = virusScan ?? throw new ArgumentNullException(nameof(virusScan));
            _comsService = comsService ?? throw new ArgumentNullException(nameof(comsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<VirusScanDocument> context)
        {
            using var scope = _logger.BeginConsumeScope(context);

            _logger.LogDebug("Consuming VirusScanDocument message");
            var cancellationToken = context.CancellationToken;

            Guid documentId = context.Message.DocumentId;

            try
            {
                // Get the file to scan from COMS
                Coms.Client.File file = await _comsService.GetFileAsync(documentId, cancellationToken);

                var stream = file.Data;
                var parameter = new Common.OpenAPIs.VirusScan.V1.FileParameter(stream);

                // Virus scan the file and get scan result
                _logger.LogDebug("Sending the file: {fileId} for virus scan", file.Id);
                VirusScanResult scanResult = await _virusScan.VirusScanAsync(parameter, cancellationToken);

                if (scanResult.Status == VirusScanStatus.NotInfected)
                {
                    _logger.LogDebug("No viruses detected as a result of the scan");
                    // Add a "clean" metadata tag to the document if no viruses detected
                    file.Metadata["virus-scan-status"] = "clean";
                    // In case the document re-scanned and it used to be infected then remove virus-name
                    file.Metadata.Remove("virus-name");
                }
                else if (scanResult.Status == VirusScanStatus.Infected)
                {
                    string virusName = scanResult.VirusName;
                    _logger.LogDebug("The document with id {documentId} is infected with virus {virusName}", documentId, virusName);
                    // Virus detected so add "infected" as virus-scan-status metadata as well as the virus name to the document
                    file.Metadata["virus-scan-status"] = "infected";
                    file.Metadata["virus-name"] = virusName;
                }
                else if (scanResult.Status == VirusScanStatus.Error)
                {
                    _logger.LogDebug("Could not determine the virus status of the document");
                    string status = scanResult.Status.ToString();
                    file.Metadata["virus-scan-status"] = "error";
                    file.Metadata.Remove("virus-name");
                }
                else
                {
                    // unknown status
                    _logger.LogWarning("Unknown virus scan status {Status}, file metadata will not be updated.", scanResult.Status);
                    return;
                }

                // Update document's metadata
                await _comsService.SetFileMetadataAsync(documentId, file.Metadata, cancellationToken);
            }
            catch (ObjectManagementServiceException e)
            {
                _logger.LogError(e, "Error retrieving the document from COMS");
                throw;
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "Could not scan the file for viruses");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting the document from COMS and scanning for viruses");
                throw;
            }
        }
    }
}
