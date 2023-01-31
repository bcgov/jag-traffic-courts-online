using MassTransit;
using System.Diagnostics;
using System.Reflection.Metadata;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.VirusScan.V1.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Consumer for DocumentUploaded message.
    /// </summary>
    public class ScanUploadedDocumentForVirusesConsumer : IConsumer<DocumentUploaded>
    {
        private readonly IVirusScanClient _virusScan;
        private readonly IWorkflowDocumentService _documentService;
        private readonly ILogger<ScanUploadedDocumentForVirusesConsumer> _logger;

        public ScanUploadedDocumentForVirusesConsumer(IVirusScanClient virusScan, IWorkflowDocumentService comsService, ILogger<ScanUploadedDocumentForVirusesConsumer> logger)
        {
            _virusScan = virusScan ?? throw new ArgumentNullException(nameof(virusScan));
            _documentService = comsService ?? throw new ArgumentNullException(nameof(comsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<DocumentUploaded> context)
        {
            using var scope = _logger.BeginConsumeScope(context);

            _logger.LogDebug("Consuming VirusScanDocument message");
            var cancellationToken = context.CancellationToken;

            Guid documentId = context.Message.Id;

            // get the file
            Coms.Client.File file = await GetFileAsync(documentId, cancellationToken);
            Debug.Assert(file.Id == documentId);

            // scan the file for viruses
            VirusScanResult scanResult = await ScanFileForVirusesAsync(file, cancellationToken);

            // check if we should update the meta data
            if (!UpdateMetadata(scanResult, file))
            {
                return;
            }

            // Update document's metadata
            await SaveFileMetadataAsync(documentId, file.Metadata, cancellationToken);
        }

        private async Task<Coms.Client.File> GetFileAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                // Get the file to scan from COMS
                Coms.Client.File file = await _documentService.GetFileAsync(id, cancellationToken);
                return file;
            }
            catch (ObjectManagementServiceException exception)
            {
                _logger.LogError(exception, "Error retrieving file from document service");
                throw new DocumentVirusScanConsumerException("Error retrieving file from document service", exception);
            }
        }

        private async Task<VirusScanResult> ScanFileForVirusesAsync(Coms.Client.File file, CancellationToken cancellationToken)
        {
            try
            {
                var stream = file.Data;
                var parameter = new Common.OpenAPIs.VirusScan.V1.FileParameter(stream);

                // Virus scan the file and get scan result
                _logger.LogDebug("Sending the file: {fileId} for virus scan", file.Id);
                VirusScanResult scanResult = await _virusScan.VirusScanAsync(parameter, cancellationToken);

                _logger.LogDebug("Virus scan result {VirusScanResult}", scanResult);

                return scanResult;
            }
            catch (ApiException exception)
            {
                _logger.LogError(exception, "Error calling virus scan service");
                throw new DocumentVirusScanConsumerException("Error calling virus scan service", exception);
            }
        }

        private bool UpdateMetadata(VirusScanResult scanResult, Coms.Client.File file)
        {
            if (scanResult.Status == VirusScanStatus.NotInfected)
            {
                _logger.LogDebug("No viruses detected as a result of the scan");
                // Add a "clean" metadata tag to the document if no viruses detected
                file.Metadata["virus-scan-status"] = "clean";
                // In case the document re-scanned and it used to be infected then remove virus-name
                file.Metadata.Remove("virus-name");
                return true;
            }
            else if (scanResult.Status == VirusScanStatus.Infected)
            {
                string virusName = scanResult.VirusName;
                _logger.LogDebug("The document with id {documentId} is infected with virus {virusName}", file.Id, virusName);
                // Virus detected so add "infected" as virus-scan-status metadata as well as the virus name to the document
                file.Metadata["virus-scan-status"] = "infected";
                file.Metadata["virus-name"] = virusName;
                return true;
            }
            else if (scanResult.Status == VirusScanStatus.Error)
            {
                _logger.LogDebug("Could not determine the virus status of the document");
                string status = scanResult.Status.ToString();
                file.Metadata["virus-scan-status"] = "error";
                file.Metadata.Remove("virus-name");
                return true;
            }

            // unknown status
            _logger.LogWarning("Unknown virus scan status {Status}, file metadata will not be updated.", scanResult.Status);
            return false; // did not update metadata
        }

        private async Task SaveFileMetadataAsync(Guid id, IReadOnlyDictionary<string, string> meta, CancellationToken cancellationToken)
        {
            try
            {
                await _documentService.SetFileMetadataAsync(id, meta, cancellationToken);
            }
            catch (ObjectManagementServiceException exception)
            {
                _logger.LogError(exception, "Error saving file meta data in document service");
                throw new DocumentVirusScanConsumerException("Error saving file meta data in document service", exception);
            }
        }
    }
}
