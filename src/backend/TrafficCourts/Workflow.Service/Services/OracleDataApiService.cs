using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services;

public class OracleDataApiService : IOracleDataApiService
{
    /// <summary>
    /// The format of a Guid when passed to Oracle data api
    /// 32 digits separated by hyphens: 00000000-0000-0000-0000-000000000000
    /// </summary>
    private const string NoticeOfDisputeGuidFormat = "d";

    private readonly IOracleDataApiClient _client;

    public OracleDataApiService(IOracleDataApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<long> CreateDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        // stub out the ViolationTicket if the submitted Dispute has associated OCR scan results.
        if (!string.IsNullOrEmpty(dispute.OcrTicketFilename))
        {
            dispute.ViolationTicket = CreateViolationTicketFromDispute(dispute);

            // TODO: initialize ViolationTicket with data from OCR 
        }

        var response = await _client.SaveDisputeAsync(dispute, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        var response = await _client.InsertFileHistoryAsync(fileHistory, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken)
    {
        var response = await _client.InsertEmailHistoryAsync(emailHistory, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<Dispute?> GetDisputeByNoticeOfDisputeGuidAsync(Guid noticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        string id = noticeOfDisputeGuid.ToString(NoticeOfDisputeGuidFormat);
     
        try
        {
            var response = await _client.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (ApiException e) when (e.StatusCode == 404)
        {
            // 404 is ok, do not end in exception
            return null;
        }
    }

    public async Task VerifyDisputeEmailAsync(long disputeId, CancellationToken cancellationToken)
    {
        await _client.VerifyDisputeEmailAsync(disputeId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Dispute> ResetDisputeEmailAsync(long disputeId, string emailAddress, CancellationToken cancellationToken)
    {
        var response = await _client.ResetDisputeEmailAsync(disputeId, emailAddress, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<IList<DisputeResult>> SearchDisputeAsync(string? ticketNumber, string? issuedTime, Guid? noticeOfDisputeGuid, ExcludeStatus2? excludeStatus, CancellationToken cancellationToken)
    {
        // need to search by ticket number and issue time, or noticeOfDisputeGuid

        if (ticketNumber is null && issuedTime is null && noticeOfDisputeGuid is null && excludeStatus is null)
        {
            // no values passed for searching
            return Array.Empty<DisputeResult>();
        }

        var noticeOfDisputeId = noticeOfDisputeGuid?.ToString(NoticeOfDisputeGuidFormat);

        var response = await _client.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeId, excludeStatus, cancellationToken).ConfigureAwait(false);
        return new List<DisputeResult>(response);
    }

    public async Task<Dispute> GetDisputeByIdAsync(long disputeId, bool isAssign, CancellationToken cancellationToken)
    {
        var response = await _client.GetDisputeAsync(disputeId, isAssign, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest disputeUpdateRequest, CancellationToken cancellationToken)
    {
        var response = await _client.SaveDisputeUpdateRequestAsync(guid, disputeUpdateRequest, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        var response = await _client.UpdateDisputeAsync(disputeId, dispute, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<ICollection<JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, CancellationToken cancellationToken)
    {
        var response = await _client.GetJJDisputesAsync(jjAssignedTo, ticketNumber, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        var response = await _client.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long disputeUpdateRequestId, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken)
    {
        var response = await _client.UpdateDisputeUpdateRequestStatusAsync(disputeUpdateRequestId, disputeUpdateRequestStatus, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long disputeId, Status? disputeUpdateRequestStatus, CancellationToken cancellationToken)
    {
        var response = await _client.GetDisputeUpdateRequestsAsync(disputeId, disputeUpdateRequestStatus, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private static ViolationTicket CreateViolationTicketFromDispute(Dispute dispute)
    {
        ViolationTicket violationTicket = new();
        violationTicket.TicketNumber = dispute.TicketNumber;
        // Stub out the violationsTicketCounts with default count no for mapping dispute counts properly in Oracle API
        List<ViolationTicketCount> violationTicketCounts = new();
        for (int i = 1; i <= 3; i++)
        {
            violationTicketCounts.Add(new ViolationTicketCount { CountNo = i });
        }
        violationTicket.ViolationTicketCounts = violationTicketCounts;

        return violationTicket;
    }
}
