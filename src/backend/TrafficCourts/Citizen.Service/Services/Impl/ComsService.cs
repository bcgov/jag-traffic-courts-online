using MassTransit;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Services.Impl;

/// <summary>
/// A service for file operations utilizing common object management service client
/// </summary>
public class ComsService : IComsService
{
    private readonly IObjectManagementService _objectManagementService;
    private readonly IMemoryStreamManager _memoryStreamManager;
    private readonly IBus _bus;
    private readonly ILogger<ComsService> _logger;

    public ComsService(
        IObjectManagementService objectManagementService,
        IMemoryStreamManager memoryStreamManager,
        IBus bus,
        ILogger<ComsService> logger)
    {
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting the file through COMS");

        // find the file so we can get the ticket number and notice of dispute id
        FileSearchParameters searchParameters = new(fileId);

        IList<FileSearchResult> searchResults = await _objectManagementService.FileSearchAsync(searchParameters, cancellationToken);

        if (searchResults.Count == 0)
        {
            return; // file not found
        }

        FileSearchResult file = searchResults[0];

        file.Metadata.TryGetValue("notice-of-dispute-id", out string? noticeOfDisputeId);
        if (string.IsNullOrEmpty(noticeOfDisputeId))
        {
            _logger.LogDebug("notice-of-dispute-id value from metadata is empty. Cannot delete the file since it was not uploaded by a disputant");
            return;
        }

        file.Metadata.TryGetValue("ticket-number", out string? ticketNumber);
        if (string.IsNullOrEmpty(ticketNumber))
        {
            ticketNumber = "unknown";
            _logger.LogDebug("ticket-number value from metadata is empty");
        }

        await _objectManagementService.DeleteFileAsync(fileId, cancellationToken);

        // Save file delete event to file history
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = ticketNumber;
        fileHistoryRecord.Description = $"File: {file.FileName} was deleted by the Disputant.";
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task<Guid> SaveFileAsync(IFormFile file, Dictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Saving file through COMS");

        metadata.Add("staff-review-status", "pending");

        using Coms.Client.File comsFile = new(GetStreamForFile(file), file.FileName, file.ContentType, metadata, null);

        Guid id = await _objectManagementService.CreateFileAsync(comsFile, cancellationToken);

        metadata.TryGetValue("ticket-number", out string? ticketNumber);
        if (string.IsNullOrEmpty(ticketNumber))
        {
            ticketNumber = "unknown";
            _logger.LogDebug("ticket-number value from metadata is empty");
        }

        // TODO: Publish a message to virus scan the uploaded document when the virus scan consumer would be added

        // Save file upload event to file history
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = ticketNumber;
        fileHistoryRecord.Description = $"File: {file.FileName} was uploaded by the Disputant.";
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
}
