using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
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
        private readonly IVirusScanService _virusScanService;
        private readonly IComsService _comsService;
        private readonly ILogger<VirusScanDocumentConsumer> _logger;

        public VirusScanDocumentConsumer(IVirusScanService virusScanService, IComsService comsService, ILogger<VirusScanDocumentConsumer> logger)
        {
            _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
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
                // Reset position to the beginning of the stream
                stream.Position = 0;

                // Virus scan the file and get scan result
                _logger.LogDebug("Sending the file: {fileId} for virus scan", file.Id);
                ScanResponse scanResult = await _virusScanService.ScanDocumentAsync(stream, cancellationToken);

                if (VirusScanStatus.NotInfected.Equals(scanResult.Status))
                {
                    _logger.LogDebug("No viruses detected as a result of the scan");
                    // Add a "clean" metadata tag to the document if no viruses detected
                    file.Metadata.Add("virus-scan-status", "clean");
                }
                else if (VirusScanStatus.Infected.Equals(scanResult.Status))
                {
                    string virusName = scanResult.VirusName;
                    _logger.LogDebug("The document with id {documentId} is infected with virus {virusName}", documentId, virusName);
                    // Virus detected so add "infected" as virus-scan-status metadata as well as the virus name to the document
                    file.Metadata.Add("virus-scan-status", "infected");
                    file.Metadata.Add("virus-name", virusName);

                    // TODO: Determine when/how to remove infected files from the storage
                }
                else
                {
                    _logger.LogDebug("Could not determine the virus status of the document");
                    string status = scanResult.Status.ToString();
                    file.Metadata.Add("virus-scan-status", status);
                }
                // Update document's metadata
                await _comsService.UpdateFileAsync(documentId, file, cancellationToken);
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
