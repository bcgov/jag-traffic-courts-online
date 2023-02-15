using MassTransit;
using TrafficCourts.Common.Models;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;

namespace TrafficCourts.Staff.Service.Services;



/// <summary>
/// A service for file operations utilizing common object management service client
/// </summary>
public class StaffDocumentService : IStaffDocumentService
{
    private readonly IObjectManagementService _objectManagementService;
    private readonly IMemoryStreamManager _memoryStreamManager;
    private readonly IBus _bus;
    private readonly ILogger<StaffDocumentService> _logger;

    public StaffDocumentService(
        IObjectManagementService objectManagementService,
        IMemoryStreamManager memoryStreamManager,
        IBus bus,
        ILogger<StaffDocumentService> logger)
    {
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting the file through COMS");

        Coms.Client.File file = await _objectManagementService.GetFileAsync(fileId, cancellationToken);

        if (!file.VirusScanIsClean())
        {
            string scanStatus = file.GetVirusScanStatus();
            _logger.LogInformation("Cannot download file that has not been successfully scanned for viruses, status is {VirusScanStatus}, expected clean.", scanStatus);
            throw new ObjectManagementServiceException($"File could not be downloaded due to virus scan status. Virus scan status of the file is {scanStatus}");
        }

        return file;
    }

    public async Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting the file through COMS");

        // find the file so we can get the ticket number
        FileSearchParameters searchParameters = new FileSearchParameters(fileId);

        IList<FileSearchResult> searchResults = await _objectManagementService.FileSearchAsync(searchParameters, cancellationToken);

        if (searchResults.Count == 0)
        {
            _logger.LogInformation("File {FileId} not found", fileId);
            return; // file not found
        }

        FileSearchResult file = searchResults[0];

        string ticketNumber = GetTicketNumber(file);

        await _objectManagementService.DeleteFileAsync(fileId, cancellationToken);

        // Save file delete event to file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, $"File: {file.FileName} was deleted by the Staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task<List<FileMetadata>> GetFilesBySearchAsync(IDictionary<string, string>? metadata, IDictionary<string, string>? tags, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Searching files through COMS");

        FileSearchParameters searchParameters = new(null, metadata, tags);

        IList<FileSearchResult> searchResult = await _objectManagementService.FileSearchAsync(searchParameters, cancellationToken);

        List<FileMetadata> fileData = new();

        foreach (var result in searchResult)
        {
            FileMetadata fileMetadata = new()
            {
                FileId = result.Id,
                FileName = result.FileName
            };

            fileData.Add(fileMetadata);
        }

        return fileData;
    }

    public async Task<Guid> SaveFileAsync(IFormFile file, Dictionary<string, string> properties, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Saving file through COMS");

        using Coms.Client.File comsFile = new(GetStreamForFile(file), file.FileName, file.ContentType, null, properties);

        Guid id = await _objectManagementService.CreateFileAsync(comsFile, cancellationToken);

        // Publish a message to virus scan the newly uploaded file
        DocumentUploaded virusScan = new()
        {
            Id = id
        };
        await _bus.PublishWithLog(_logger, virusScan, cancellationToken);

        string ticketNumber = GetTicketNumber(properties);
        
        // Save file upload event to file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, $"File: {file.FileName} was uploaded by the Staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

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

    private string GetTicketNumber(FileSearchResult file)
    {
        string? ticketNumber = file.GetTicketNumber();
        if (string.IsNullOrEmpty(ticketNumber))
        {
            ticketNumber = "unknown";
            _logger.LogDebug("ticket-number value from metadata is empty");
        }

        return ticketNumber;
    }

    private string GetTicketNumber(Dictionary<string, string> properties)
    {
        string? ticketNumber = properties.GetTicketNumber();
        if (string.IsNullOrEmpty(ticketNumber))
        {
            ticketNumber = "unknown";
            _logger.LogDebug("ticket-number value from metadata is empty");
        }

        return ticketNumber;
    }
}
