using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class DisputeService : IDisputeService
{
    private readonly ILogger<DisputeService> _logger;
    private readonly ICancelledDisputeEmailTemplate _cancelledDisputeEmailTemplate;
    private readonly IRejectedDisputeEmailTemplate _rejectedDisputeEmailTemplate;
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IBus _bus;
    private readonly IFilePersistenceService _filePersistenceService;

    public DisputeService(
        IOracleDataApiClient oracleDataApi,
        IBus bus,
        IFilePersistenceService filePersistenceService,
        ILogger<DisputeService> logger,
        ICancelledDisputeEmailTemplate cancelledDisputeEmailTemplate,
        IRejectedDisputeEmailTemplate rejectedDisputeEmailTemplate)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cancelledDisputeEmailTemplate = cancelledDisputeEmailTemplate;
        _rejectedDisputeEmailTemplate = rejectedDisputeEmailTemplate;
    }

    public async Task<ICollection<Dispute>> GetAllDisputesAsync(ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetAllDisputesAsync(null, excludeStatus, cancellationToken);
    }

    public async Task<long> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);

        // Try to retrieve OCR Violation Ticket Data from object storage for the given NoticeOfDisputeGuid (filename)
        OcrViolationTicket? ocrViolationTicket = await _filePersistenceService.GetJsonDataAsync<OcrViolationTicket>(dispute.NoticeOfDisputeGuid, cancellationToken);
        dispute.ViolationTicket.OcrViolationTicket = ocrViolationTicket;

        // If OcrViolationTicket != null, then this Violation Ticket was scanned using the Azure OCR Form Recognizer at one point.
        // If so, retrieve the image from object storage and return it as well.
        if (ocrViolationTicket != null)
        {
            dispute.ViolationTicket.ViolationTicketImage = await GetViolationTicketImageAsync(ocrViolationTicket.ImageFilename, cancellationToken);
        }
        
        // deserialize json string to violation ticket fields
        if (dispute.OcrViolationTicket != null) dispute.ViolationTicket.OcrViolationTicket = System.Text.Json.JsonSerializer.Deserialize<OcrViolationTicket>(dispute.OcrViolationTicket);

        return dispute;
    }

    /// <summary>
    /// Extracts the path to the image located in the object store from the dispute record, or null if not filename could be found.
    /// </summary>
    /// <param name="dispute"></param>
    /// <returns></returns>
    public string? GetViolationTicketImageFilename(Dispute dispute)
    {
        if (dispute.OcrViolationTicket is not null)
        {
            try
            {
                JsonElement element = JsonSerializer.Deserialize<JsonElement>(dispute.OcrViolationTicket);
                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("ImageFilename", out JsonElement imageFilename))
                {
                    return imageFilename.GetString();
                }
            }
            catch (Exception ex)
            {
                // Should never reach here, but if so then it means the ocr json data is invalid or not parseable by .NET
                // For now, just log the error and return null to mean no image could be found so the GetDispute(id) endpoint doesn't break.
                _logger.LogError(ex, "Could not extract object store file reference from json data");
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves a image from the object store with the given imageFilename.
    /// </summary>
    /// <param name="imageFilename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<ViolationTicketImage?> GetViolationTicketImageAsync(string? imageFilename, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(imageFilename))
        {
            return null;
        }

        using (_logger.BeginScope(new Dictionary<string, object> { ["FileName"] = imageFilename }))
        {
            try
            {
                MemoryStream stream = await _filePersistenceService.GetFileAsync(imageFilename, cancellationToken);
                FileMimeType? mimeType = stream.GetMimeType();
                if (mimeType is null)
                {
                    _logger.LogWarning("Could not determine mime type for file");
                    return null;
                }

                return new ViolationTicketImage(stream.ToArray(), mimeType.MimeType);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not retrieve image from object storage");
                throw;
            }
        }
    }

    public async Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
    }

    public async Task ValidateDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute status setting to validated");

        Dispute dispute = await _oracleDataApi.ValidateDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(dispute.TicketNumber, "Handwritten ticket OCR details validated by staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task CancelDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute cancelled");

        Dispute dispute = await _oracleDataApi.CancelDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(dispute.TicketNumber, "Dispute cancelled by staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeCancelled cancelledEvent = Mapper.ToDisputeCancelled(dispute);
        await _bus.PublishWithLog(_logger, cancelledEvent, cancellationToken);

        var emailMessage = _cancelledDisputeEmailTemplate.Create(dispute);
        await _bus.PublishWithLog(_logger, emailMessage, cancellationToken);
    }

    public async Task RejectDisputeAsync(long disputeId, string rejectedReason, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute rejected");

        Dispute dispute = await _oracleDataApi.RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(dispute.TicketNumber, "Dispute rejected by staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeRejected rejectedEvent = Mapper.ToDisputeRejected(dispute);
        await _bus.PublishWithLog(_logger, rejectedEvent, cancellationToken);

        var emailMessage = _rejectedDisputeEmailTemplate.Create(dispute);
        await _bus.PublishWithLog(_logger, emailMessage, cancellationToken);
    }

    public async Task SubmitDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute submitted for approval processing");

        // Save and status to PROCESSING
        Dispute dispute = await _oracleDataApi.SubmitDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(dispute.TicketNumber, "Dispute submitted to ARC by staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will push event to ARC and generate email)
        DisputeApproved approvedEvent = Mapper.ToDisputeApproved(dispute);
        await _bus.PublishWithLog(_logger, approvedEvent, cancellationToken);
    }

    public async Task DeleteDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        await _oracleDataApi.DeleteDisputeAsync(disputeId, cancellationToken);
    }

    public async Task<string> ResendEmailVerificationAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Email verification sent");

        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        EmailVerificationSend emailVerificationSentEvent = Mapper.ToEmailVerification(new Guid(dispute.NoticeOfDisputeGuid));
        await _bus.PublishWithLog(_logger, emailVerificationSentEvent, cancellationToken);

        return "Email verification sent";
    }
}
