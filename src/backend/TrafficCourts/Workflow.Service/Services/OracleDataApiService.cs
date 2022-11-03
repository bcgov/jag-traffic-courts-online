using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services;

public class OracleDataApiService : IOracleDataApiService
{
    /// <summary>
    /// The format of a Guid when passed to Oracle data api
    /// 32 digits separated by hyphens: 00000000-0000-0000-0000-000000000000
    /// </summary>
    private const string NoticeOfDisputeIdFormat = "d";

    private readonly IOracleDataApiClient _client;

    public OracleDataApiService(IOracleDataApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<long> CreateDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        // stub out the ViolationTicket if the submitted Dispute has associated OCR scan results.
        if (!string.IsNullOrEmpty(dispute.OcrViolationTicket))
        {
            dispute.ViolationTicket = new();
            dispute.ViolationTicket.TicketNumber = dispute.TicketNumber;

            // TODO: initialize ViolationTicket with data from OCR 
        }

        return await _client.SaveDisputeAsync(dispute, cancellationToken);
    }
    public async Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.InsertFileHistoryAsync(fileHistory.TicketNumber, fileHistory, cancellationToken);
        } 
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.InsertEmailHistoryAsync(emailHistory.TicketNumber, emailHistory, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dispute?> GetDisputeByNoticeOfDisputeIdAsync(Guid noticeOfDisputeId, CancellationToken cancellationToken)
    {
        var id = noticeOfDisputeId.ToString(NoticeOfDisputeIdFormat);
        try
        {
            return await _client.GetDisputeByNoticeOfDisputeIdAsync(id, cancellationToken);
        }
        catch (ApiException e) when (e.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task VerifyDisputeEmailAsync(long disputeId, CancellationToken cancellationToken)
    {
        await _client.VerifyDisputeEmailAsync(disputeId);
    }
}
