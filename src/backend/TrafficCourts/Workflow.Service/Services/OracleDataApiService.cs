using System.Threading;
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
            dispute.ViolationTicket = new();
            dispute.ViolationTicket.TicketNumber = dispute.TicketNumber;
            // Stub out the violationsTicketCounts with default count no for mapping dispute counts properly in Oracle API
            List<ViolationTicketCount> violationTicketCounts = new();
            for (int i = 1; i <= 3; i++)
            {
                ViolationTicketCount violationTicketCount = new();
                violationTicketCount.CountNo = i;
                violationTicketCounts.Add(violationTicketCount);
            }
            dispute.ViolationTicket.ViolationTicketCounts = violationTicketCounts;

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
        await _client.VerifyDisputeEmailAsync(disputeId);
    }

    public async Task<ICollection<DisputeResult>> SearchDisputeAsync(string ticketNumber, string issuedTime, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.FindDisputeAsync(ticketNumber, issuedTime, cancellationToken);
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

    public async Task<DisputantUpdateRequest> UpdateDisputantUpdateRequestStatusAsync(long disputantUpdateRequestId, DisputantUpdateRequestStatus disputantUpdateRequestStatus, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.UpdateDisputantUpdateRequestStatusAsync(disputantUpdateRequestId, disputantUpdateRequestStatus, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
