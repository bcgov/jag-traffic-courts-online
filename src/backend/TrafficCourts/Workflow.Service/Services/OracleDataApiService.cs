using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services;

public class OracleDataApiService : IOracleDataApiService
{
    private readonly IOracleDataApiClient _oracleDataApiClient;

    public OracleDataApiService(IOracleDataApiClient oracleDataApiClient)
    {
        _oracleDataApiClient = oracleDataApiClient ?? throw new ArgumentNullException(nameof(oracleDataApiClient));
    }

    public async Task<long> CreateDisputeAsync(Dispute dispute)
    {
        // stub out the ViolationTicket if the submitted Dispute has associated OCR scan results.
        if (!string.IsNullOrEmpty(dispute.OcrViolationTicket))
        {
            dispute.ViolationTicket = new();

            // TODO: initialize ViolationTicket with data from OCR 
        }

        return await _oracleDataApiClient.SaveDisputeAsync(dispute);
    }
    public async Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory)
    {
        return await _oracleDataApiClient.InsertEmailHistoryAsync(emailHistory.TicketNumber, emailHistory);
    }


    public async Task<Dispute> GetDisputeByEmailVerificationTokenAsync(string emailVerificationToken)
    {
        return await _oracleDataApiClient.GetDisputeByEmailVerificationTokenAsync(emailVerificationToken);
    }
}
