using MediatR;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Features.Caching;
using TrafficCourts.Staff.Service.Features.Caching.DisputeChanged;
using TrafficCourts.Staff.Service.Models;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Services
{
    public class OracleDataApiService : IOracleDataApiService
    {
        private readonly IOracleDataApiClient _client;
        private readonly IFusionCache _cache;
        private readonly IMediator _mediator;

        public OracleDataApiService(IOracleDataApiClient client, IFusionCache cache, IMediator mediator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId, CancellationToken cancellationToken)
        {
            var result = await _client.AcceptJJDisputeAsync(ticketNumber, checkVTCAssigned, partId, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task AssignJJDisputesToJJAsync(IEnumerable<string> ticketNumbers, string? jjUsername, CancellationToken cancellationToken)
        {
            await _client.AssignJJDisputesToJJAsync(ticketNumbers, jjUsername, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
        }

        public async Task<Dispute> CancelDisputeAsync(long id, string body, CancellationToken cancellationToken)
        {
            var result = await _client.CancelDisputeAsync(id, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken)
        {
            var result = await _client.CancelJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task CodeTableRefreshAsync(CancellationToken cancellationToken)
        {
            await _client.CodeTableRefreshAsync(cancellationToken);
        }

        public async Task<JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken)
        {
            var result = await _client.ConcludeJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            var result = await _client.ConfirmJJDisputeAsync(ticketNumber, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task DeleteDisputeAsync(long id, CancellationToken cancellationToken)
        {
            await _client.DeleteDisputeAsync(id, cancellationToken);
            await DisputesChangedAsync(cancellationToken);

        }

        public async Task DeleteJJDisputeAsync(long? jjDisputeId, string ticketNumber, CancellationToken cancellationToken)
        {
            await _client.DeleteJJDisputeAsync(jjDisputeId, ticketNumber, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
        }

        public async Task<ICollection<DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string? issuedTime, string? noticeOfDisputeGuid, CancellationToken cancellationToken)
        {
            var result = await _client.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, cancellationToken);

            return result;
        }

        public async Task<ICollection<DisputeListItem>> GetAllDisputesAsync(DateTimeOffset? newerThan, ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
        {
            // try to fetch from the cache without the parameters
            ICollection<DisputeListItem> disputes = await _cache.GetOrSetAsync<ICollection<DisputeListItem>>(
                Keys.Dispute.DisputeListItems,
                ct => _client.GetAllDisputesAsync(null, null, ct),
                options => options.SetDuration(TimeSpan.FromMinutes(1)),
                token: cancellationToken);

            if (newerThan is not null || excludeStatus is not null)
            {
                disputes = disputes
                    .Filter(new GetAllDisputesParameters { ExcludeStatus = excludeStatus, From = newerThan })
                    .ToList();
            }

            return disputes;
        }

        public async Task<Dispute> GetDisputeAsync(long id, bool isAssign, CancellationToken cancellationToken)
        {
            var result = await _client.GetDisputeAsync(id, isAssign, cancellationToken);
            return result;
        }

        public async Task<Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _client.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken);
            return result;
        }

        public async Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, Status? status, CancellationToken cancellationToken)
        {
            var result = await _client.GetDisputeUpdateRequestsAsync(id, status, cancellationToken);
            return result;
        }

        public async Task<ICollection<EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            var result = await _client.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
            return result;
        }

        public async Task<ICollection<FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            var result = await _client.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
            return result;
        }

        public async Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
        {
            var result = await _client.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken);
            return result;
        }

        public async Task<ICollection<JJDispute>> GetJJDisputesAsync(string? jjAssignedTo, string? ticketNumber, CancellationToken cancellationToken)
        {
            var result = await _client.GetJJDisputesAsync(jjAssignedTo, ticketNumber, cancellationToken);
            return result;
        }

        public async Task<TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken)
        {
            var result = await _client.GetTicketImageDataAsync(ticketNumber, documentType, cancellationToken);
            return result;
        }

        public async Task<long> InsertEmailHistoryAsync(EmailHistory body, CancellationToken cancellationToken)
        {
            var result = await _client.InsertEmailHistoryAsync(body, cancellationToken);
            return result;
        }

        public async Task<long> InsertFileHistoryAsync(FileHistory body, CancellationToken cancellationToken)
        {
            var result = await _client.InsertFileHistoryAsync(body, cancellationToken);
            return result;
        }

        public async Task<Dispute> RejectDisputeAsync(long id, string body, CancellationToken cancellationToken)
        {
            var result = await _client.RejectDisputeAsync(id, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, CancellationToken cancellationToken)
        {
            var result = await _client.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<Dispute> ResetDisputeEmailAsync(long id, string email, CancellationToken cancellationToken)
        {
            var result = await _client.ResetDisputeEmailAsync(id, email, cancellationToken);
            return result;
        }

        public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body, CancellationToken cancellationToken)
        {
            var result = await _client.ReviewJJDisputeAsync(ticketNumber, checkVTCAssigned, recalled, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<long> SaveDisputeAsync(Dispute body, CancellationToken cancellationToken)
        {
            var result = await _client.SaveDisputeAsync(body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest body, CancellationToken cancellationToken)
        {
            var result = await _client.SaveDisputeUpdateRequestAsync(guid, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<Dispute> SubmitDisputeAsync(long id, CancellationToken cancellationToken)
        {
            var result = await _client.SubmitDisputeAsync(id, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task UnassignDisputesAsync(CancellationToken cancellationToken)
        {
            await _client.UnassignDisputesAsync(cancellationToken);
            await DisputesChangedAsync(cancellationToken);
        }

        public async Task<Dispute> UpdateDisputeAsync(long id, Dispute body, CancellationToken cancellationToken)
        {
            var result = await _client.UpdateDisputeAsync(id, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);

            return result;
        }

        public async Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken)
        {
            var result = await _client.UpdateDisputeUpdateRequestStatusAsync(id, disputeUpdateRequestStatus, cancellationToken);
            return result;
        }

        public async Task<JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken)
        {
            var result = await _client.UpdateJJDisputeAsync(ticketNumber, checkVTCAssigned, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken)
        {
            var result = await _client.UpdateJJDisputeCascadeAsync(ticketNumber, checkVTCAssigned, body, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task<Dispute> ValidateDisputeAsync(long id, CancellationToken cancellationToken)
        {
            var result = await _client.ValidateDisputeAsync(id, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
            return result;
        }

        public async Task VerifyDisputeEmailAsync(long id, CancellationToken cancellationToken)
        {
            await _client.VerifyDisputeEmailAsync(id, cancellationToken);
            await DisputesChangedAsync(cancellationToken);
        }

        private async Task DisputesChangedAsync(CancellationToken cancellationToken)
        {
            await _mediator.Publish(DisputeChangedNotification.Instance, cancellationToken);
        }
    }
}
