using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0
{
    [OperationTimer]
    public partial class InstrumentedOracleDataApiClient : IOracleDataApiClient
    {
        private readonly IOracleDataApiClient _inner;
        private readonly IOperationMetrics _metrics;

        public InstrumentedOracleDataApiClient(IOracleDataApiClient inner, IOperationMetrics metrics)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.AcceptJJDisputeAsync(ticketNumber, checkVTCAssigned, partId).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.AcceptJJDisputeAsync(ticketNumber, checkVTCAssigned, partId, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task AssignJJDisputesToJJAsync(System.Collections.Generic.IEnumerable<string> ticketNumbers, string jjUsername)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.AssignJJDisputesToJJAsync(ticketNumbers, jjUsername).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task AssignJJDisputesToJJAsync(System.Collections.Generic.IEnumerable<string> ticketNumbers, string jjUsername, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.AssignJJDisputesToJJAsync(ticketNumbers, jjUsername, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> CancelDisputeAsync(long id, string body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.CancelDisputeAsync(id, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> CancelDisputeAsync(long id, string body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.CancelDisputeAsync(id, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.CancelJJDisputeAsync(ticketNumber, checkVTCAssigned).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.CancelJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task CodeTableRefreshAsync()
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.CodeTableRefreshAsync().ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task CodeTableRefreshAsync(System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.CodeTableRefreshAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ConcludeJJDisputeAsync(ticketNumber, checkVTCAssigned).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ConcludeJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ConfirmJJDisputeAsync(string ticketNumber)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ConfirmJJDisputeAsync(ticketNumber).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ConfirmJJDisputeAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ConfirmJJDisputeAsync(ticketNumber, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task DeleteDisputeAsync(long id)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.DeleteDisputeAsync(id).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task DeleteDisputeAsync(long id, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.DeleteDisputeAsync(id, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task DeleteJJDisputeAsync(long? jjDisputeId, string ticketNumber)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.DeleteJJDisputeAsync(jjDisputeId, ticketNumber).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task DeleteJJDisputeAsync(long? jjDisputeId, string ticketNumber, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.DeleteJJDisputeAsync(jjDisputeId, ticketNumber, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string issuedTime, string noticeOfDisputeGuid)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string issuedTime, string noticeOfDisputeGuid, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeListItem>> GetAllDisputesAsync(System.DateTimeOffset? newerThan, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ExcludeStatus? excludeStatus)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetAllDisputesAsync(newerThan, excludeStatus).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeListItem>> GetAllDisputesAsync(System.DateTimeOffset? newerThan, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ExcludeStatus? excludeStatus, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetAllDisputesAsync(newerThan, excludeStatus, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> GetDisputeAsync(long id, bool isAssign)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeAsync(id, isAssign).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> GetDisputeAsync(long id, bool isAssign, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeAsync(id, isAssign, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeByNoticeOfDisputeGuidAsync(id).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Status? status)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeUpdateRequestsAsync(id, status).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Status? status, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetDisputeUpdateRequestsAsync(id, status, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetEmailHistoryByTicketNumberAsync(ticketNumber).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetFileHistoryByTicketNumberAsync(ticketNumber).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetJJDisputeAsync(ticketNumber, assignVTC).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetJJDisputesAsync(jjAssignedTo, ticketNumber).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetJJDisputesAsync(jjAssignedTo, ticketNumber, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DocumentType documentType)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetTicketImageDataAsync(ticketNumber, documentType).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DocumentType documentType, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.GetTicketImageDataAsync(ticketNumber, documentType, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> InsertEmailHistoryAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.InsertEmailHistoryAsync(body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> InsertEmailHistoryAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.InsertEmailHistoryAsync(body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> InsertFileHistoryAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.InsertFileHistoryAsync(body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> InsertFileHistoryAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.InsertFileHistoryAsync(body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> RejectDisputeAsync(long id, string body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.RejectDisputeAsync(id, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> RejectDisputeAsync(long id, string body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.RejectDisputeAsync(id, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string remark)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.RequireCourtHearingJJDisputeAsync(ticketNumber, remark).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string remark, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> ResetDisputeEmailAsync(long id, string email)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ResetDisputeEmailAsync(id, email).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> ResetDisputeEmailAsync(long id, string email, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ResetDisputeEmailAsync(id, email, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ReviewJJDisputeAsync(ticketNumber, checkVTCAssigned, recalled, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ReviewJJDisputeAsync(ticketNumber, checkVTCAssigned, recalled, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> SaveDisputeAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SaveDisputeAsync(body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> SaveDisputeAsync(TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SaveDisputeAsync(body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> SaveDisputeUpdateRequestAsync(string guid, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SaveDisputeUpdateRequestAsync(guid, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<long> SaveDisputeUpdateRequestAsync(string guid, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SaveDisputeUpdateRequestAsync(guid, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> SubmitDisputeAsync(long id)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SubmitDisputeAsync(id).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> SubmitDisputeAsync(long id, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.SubmitDisputeAsync(id, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task UnassignDisputesAsync()
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.UnassignDisputesAsync().ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task UnassignDisputesAsync(System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.UnassignDisputesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> UpdateDisputeAsync(long id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateDisputeAsync(id, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> UpdateDisputeAsync(long id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateDisputeAsync(id, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequestStatus disputeUpdateRequestStatus)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateDisputeUpdateRequestStatusAsync(id, disputeUpdateRequestStatus).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequestStatus disputeUpdateRequestStatus, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateDisputeUpdateRequestStatusAsync(id, disputeUpdateRequestStatus, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateJJDisputeAsync(ticketNumber, checkVTCAssigned, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateJJDisputeAsync(ticketNumber, checkVTCAssigned, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute body)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateJJDisputeCascadeAsync(ticketNumber, checkVTCAssigned, body).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute body, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.UpdateJJDisputeCascadeAsync(ticketNumber, checkVTCAssigned, body, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> ValidateDisputeAsync(long id)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ValidateDisputeAsync(id).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute> ValidateDisputeAsync(long id, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                var result = await _inner.ValidateDisputeAsync(id, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task VerifyDisputeEmailAsync(long id)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.VerifyDisputeEmailAsync(id).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }

        public async global::System.Threading.Tasks.Task VerifyDisputeEmailAsync(long id, System.Threading.CancellationToken cancellationToken)
        {
            var operation = _metrics.BeginOperation();

            try
            {
                await _inner.VerifyDisputeEmailAsync(id, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (global::System.Exception exception)
            {
                operation.Error(exception);
                throw;
            }
        }
    }
}
