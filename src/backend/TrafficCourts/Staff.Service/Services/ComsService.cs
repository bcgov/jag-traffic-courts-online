using MassTransit;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;

namespace TrafficCourts.Staff.Service.Services;

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

    public async Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting the file through COMS");

        Coms.Client.File comsFile = await _objectManagementService.GetFileAsync(fileId, false, cancellationToken);

        Dictionary<string, string> metadata = comsFile.Metadata;

        if (!metadata.ContainsKey("virus-scan-status"))
        {
            _logger.LogError("Could not download the document because metadata does not contain the key: virus-scan-status");
            throw new ObjectManagementServiceException("File could not be downloaded due to the missing metadata key: virus-scan-status");
        }

        metadata.TryGetValue("virus-scan-status", out string? scanStatus);
        if (!string.IsNullOrEmpty(scanStatus) && scanStatus != "clean")
        {
            _logger.LogDebug("Trying to download unscanned or virus detected file");
            throw new ObjectManagementServiceException($"File could not be downloaded due to virus scan status. Virus scan status of the file is {scanStatus}");
        }

        return comsFile;
    }

    public async Task<Guid> SaveFileAsync(IFormFile file, Dictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Saving file through COMS");

        using var stream = GetStreamForFile(file);

        Coms.Client.File comsFile = new(stream, file.FileName, file.ContentType, metadata, null);

        Guid id = await _objectManagementService.CreateFileAsync(comsFile, cancellationToken);

        metadata.TryGetValue("ticketnumber", out string? ticketNumber);
        if (string.IsNullOrEmpty(ticketNumber))
        {
            ticketNumber = "unknown";
            _logger.LogDebug("ticketnumber value from metadata is empty");
        }
        
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

}
