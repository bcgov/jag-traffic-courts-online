using MassTransit;
using TrafficCourts.Citizen.Service.Models;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Services.Impl;

/// <summary>
/// A service for file operations utilizing common object management service client
/// </summary>
public class CitizenDocumentService : ICitizenDocumentService
{
    private readonly IObjectManagementService _objectManagementService;
    private readonly IMemoryStreamManager _memoryStreamManager;
    private readonly IBus _bus;
    private readonly ILogger<CitizenDocumentService> _logger;

    public CitizenDocumentService(
        IObjectManagementService objectManagementService,
        IMemoryStreamManager memoryStreamManager,
        IBus bus,
        ILogger<CitizenDocumentService> logger)
    {
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting the file {FileId} through COMS", fileId);

        // find the file so we can get the ticket number and notice of dispute id
        FileSearchParameters searchParameters = new(fileId);

        IList<FileSearchResult> searchResults = await _objectManagementService.FileSearchAsync(searchParameters, cancellationToken);

        if (searchResults.Count == 0)
        {
            return; // file not found
        }

        FileSearchResult file = searchResults[0];

        DocumentProperties properties = new DocumentProperties(file.Metadata, file.Tags);

        if (properties.NoticeOfDisputeId is null)
        {
            _logger.LogDebug("notice-of-dispute-id value from metadata is empty. Cannot delete the file since it was not uploaded by a disputant");
            return;
        }

        await _objectManagementService.DeleteFileAsync(fileId, cancellationToken);

        // Save file delete event to file history
        SaveFileHistoryRecord fileHistoryRecord = new()
        {
            NoticeOfDisputeId = properties.NoticeOfDisputeId.Value.ToString("d"), // dashes
            ActionByApplicationUser = "Disputant",
            AuditLogEntryType = FileHistoryAuditLogEntryType.FDLD
        };

        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting the file {FileId} through COMS", fileId);

        Coms.Client.File file = await _objectManagementService.GetFileAsync(fileId, cancellationToken);

        var properties = new DocumentProperties(file.Metadata, file.Tags);

        if (!properties.VirusScanIsClean)
        {
            string scanStatus = properties.VirusScanStatus;
            _logger.LogDebug("Trying to download unscanned or virus detected file {FileId}", fileId);
            throw new ObjectManagementServiceException($"File could not be downloaded due to virus scan status. Virus scan status of the file is {scanStatus}");
        }

        return file;
    }

    public async Task<List<FileMetadata>> FindFilesAsync(DocumentProperties properties, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Searching files through COMS");

        var metadata = properties.ToMetadata();
        var tags = properties.ToTags();

        FileSearchParameters searchParameters = new(null, metadata, tags);

        IList<FileSearchResult> searchResult = await _objectManagementService.FileSearchAsync(searchParameters, cancellationToken);

        List<FileMetadata> fileData = new();

        foreach (var result in searchResult)
        {
            properties = new DocumentProperties(result.Metadata, result.Tags);

            FileMetadata fileMetadata = new()
            {
                FileId = result.Id,
                FileName = result.FileName,
                DocumentType = properties.DocumentType,
                NoticeOfDisputeGuid = properties.NoticeOfDisputeId?.ToString("d"),
                VirusScanStatus = properties.VirusScanStatus,
                DocumentStatus = properties.StaffReviewStatus,
            };

            fileData.Add(fileMetadata);
        }

        return fileData;
    }

    public async Task<Guid> SaveFileAsync(string base64FileString, string fileName, DocumentProperties properties, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Saving file through COMS");

        properties.DocumentSource = DocumentSource.Citizen;
        properties.StaffReviewStatus = "pending";

        var metadata = properties.ToMetadata();
        var tags = properties.ToTags();

        string[] fileStringSplit = base64FileString.Split(",");
        byte[] bytes = Convert.FromBase64String(fileStringSplit[1]);
        MemoryStream stream = new MemoryStream(bytes);
        //IFormFile file = new FormFile(stream, 0, bytes.Length, fileName, fileName);
        string contentType = GetStringBetween(base64FileString, "data:", ";base64");
        using Coms.Client.File comsFile = new(stream, fileName, contentType, metadata, tags);

        Guid id = await _objectManagementService.CreateFileAsync(comsFile, cancellationToken);

        // Publish a message to virus scan the newly uploaded file
        await _bus.PublishWithLog(_logger, new DocumentUploaded { Id = id }, cancellationToken);

        if (properties.NoticeOfDisputeId is null)
        {
            _logger.LogDebug("notice-of-dispute-id value from metadata is empty. Could not save document upload event to File History");
        } 
        else
        {
            // Save file upload event to file history
            SaveFileHistoryRecord fileHistoryRecord = new()
            {
                NoticeOfDisputeId = properties.NoticeOfDisputeId.Value.ToString("d"),
                ActionByApplicationUser = "Disputant",
                AuditLogEntryType = FileHistoryAuditLogEntryType.FUPD
            };
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }

        return id;
    }

    private MemoryStream GetStreamForFile(IFormFile formFile)
    {
        MemoryStream memoryStream = _memoryStreamManager.GetStream();

        using var fileStream = formFile.OpenReadStream();
        fileStream.CopyTo(memoryStream);

        // Reset position to the beginning of the stream
        memoryStream.Position = 0;

        return memoryStream;
    }

    private string GetStringBetween(string input, string startString, string endString)
    {
        int startIndex = input.IndexOf(startString) + startString.Length;
        int endIndex = input.IndexOf(endString, startIndex);

        if (startIndex < 0 || endIndex < 0 || endIndex < startIndex)
        {
            return string.Empty;
        }

        return input.Substring(startIndex, endIndex - startIndex);
    }
}
