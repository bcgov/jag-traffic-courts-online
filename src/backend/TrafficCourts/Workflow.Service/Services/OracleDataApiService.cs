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

        return await _client.SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.InsertFileHistoryAsync(fileHistory, cancellationToken);
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
            return await _client.InsertEmailHistoryAsync(emailHistory, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dispute?> GetDisputeByNoticeOfDisputeGuidAsync(Guid NoticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        var id = NoticeOfDisputeGuid.ToString(NoticeOfDisputeGuidFormat);
        try
        {
            return await _client.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken);
        }
        catch (ApiException e) when (e.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task VerifyDisputeEmailAsync(long disputeId, CancellationToken cancellationToken)
    {
        await _client.VerifyDisputeEmailAsync(disputeId, cancellationToken);
    }

    public async Task<Dispute> ResetDisputeEmailAsync(long disputeId, string emailAddress, CancellationToken cancellationToken)
    {
        return await _client.ResetDisputeEmailAsync(disputeId, emailAddress, cancellationToken);
    }

    public async Task<ICollection<DisputeResult>> SearchDisputeAsync(string? ticketNumber, string? issuedTime, string? noticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dispute> GetDisputeByIdAsync(long disputeId, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetDisputeAsync(disputeId, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest disputeUpdateRequest, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.SaveDisputeUpdateRequestAsync(guid, disputeUpdateRequest, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetJJDisputesAsync(jjAssignedTo, ticketNumber, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long disputeUpdateRequestId, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.UpdateDisputeUpdateRequestStatusAsync(disputeUpdateRequestId, disputeUpdateRequestStatus, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long disputeId, Status? disputeUpdateRequestStatus, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetDisputeUpdateRequestsAsync(disputeId, disputeUpdateRequestStatus);
        }
        catch (Exception)
        {
            throw;
        }
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
