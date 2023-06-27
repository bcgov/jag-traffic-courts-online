using MassTransit;
using System.Diagnostics;
using System.Reflection.Metadata;
using TrafficCourts.Common.Models;
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
            var newProperties = GetUpdatedDocumentProperties(scanResult, file);
            if (newProperties is null)
            {
                return; // nothing changed
            }

            // Update document's metadata
            await SaveFilePropertiesAsync(documentId, newProperties, cancellationToken);
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

        /// <summary>
        /// Gets the updated document properties or null of nothing was changed.
        /// </summary>
        /// <param name="scanResult"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private DocumentProperties? GetUpdatedDocumentProperties(VirusScanResult scanResult, Coms.Client.File file)
        {
            var properties = new DocumentProperties(file.Metadata, file.Tags);

            if (scanResult.Status == VirusScanStatus.NotInfected)
            {
                _logger.LogDebug("No viruses detected as a result of the scan");
                properties.SetVirusScanNotInfected();
                return properties;
            }
            else if (scanResult.Status == VirusScanStatus.Infected)
            {
                string virusName = scanResult.VirusName;
                _logger.LogDebug("The document with id {documentId} is infected with virus {virusName}", file.Id, virusName);
                // Virus detected so add "infected" as virus-scan-status metadata as well as the virus name to the document
                properties.SetVirusScanInfected(virusName);
                return properties;
            }
            else if (scanResult.Status == VirusScanStatus.Error)
            {
                _logger.LogDebug("Could not determine the virus status of the document");
                properties.SetVirusScanError();
                return properties;
            }

            // unknown status
            _logger.LogWarning("Unknown virus scan status {Status}, file metadata will not be updated.", scanResult.Status);
            return null; // did not update metadata
        }

        private async Task SaveFilePropertiesAsync(Guid id, DocumentProperties properties, CancellationToken cancellationToken)
        {
            try
            {
                await _documentService.SaveDocumentPropertiesAsync(id, properties, cancellationToken);
            }
            catch (ObjectManagementServiceException exception)
            {
                _logger.LogError(exception, "Error saving file meta data in document service");
                throw new DocumentVirusScanConsumerException("Error saving file meta data in document service", exception);
            }
        }
    }
}
