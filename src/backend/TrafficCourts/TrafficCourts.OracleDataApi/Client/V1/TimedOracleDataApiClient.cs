
namespace TrafficCourts.OracleDataApi.Client.V1;

/// <summary>
/// Generated type to wrap the true <see cref="IOracleDataApiClient"/> implementation.
/// </summary>
internal class TimedOracleDataApiClient : IOracleDataApiClient
{
    private readonly IOracleDataApiClient _inner;
    private readonly IOracleDataApiOperationMetrics _metrics;

    public TimedOracleDataApiClient(IOracleDataApiClient inner, IOracleDataApiOperationMetrics metrics)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> CancelDisputeAsync(long id, string body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> CancelDisputeAsync(long id, string body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ConfirmJJDisputeAsync(string ticketNumber)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ConfirmJJDisputeAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string issuedTime, string noticeOfDisputeGuid, TrafficCourts.OracleDataApi.Client.V1.ExcludeStatus2? excludeStatus)
    {
        using var operation = _metrics.BeginOperation();

        try
        {
            var result = await _inner.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, excludeStatus).ConfigureAwait(false);
            return result;
        }
        catch (global::System.Exception exception)
        {
            operation.Error(exception);
            throw;
        }
    }

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string issuedTime, string noticeOfDisputeGuid, TrafficCourts.OracleDataApi.Client.V1.ExcludeStatus2? excludeStatus, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

        try
        {
            var result = await _inner.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, excludeStatus, cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch (global::System.Exception exception)
        {
            operation.Error(exception);
            throw;
        }
    }

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeListItem>> GetAllDisputesAsync(System.DateTimeOffset? newerThan, TrafficCourts.OracleDataApi.Client.V1.ExcludeStatus? excludeStatus)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeListItem>> GetAllDisputesAsync(System.DateTimeOffset? newerThan, TrafficCourts.OracleDataApi.Client.V1.ExcludeStatus? excludeStatus, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> GetDisputeAsync(long id, bool isAssign)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> GetDisputeAsync(long id, bool isAssign, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, TrafficCourts.OracleDataApi.Client.V1.Status? status)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, TrafficCourts.OracleDataApi.Client.V1.Status? status, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<System.Collections.Generic.ICollection<TrafficCourts.OracleDataApi.Client.V1.JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, TrafficCourts.OracleDataApi.Client.V1.DocumentType documentType)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, TrafficCourts.OracleDataApi.Client.V1.DocumentType documentType, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> InsertEmailHistoryAsync(TrafficCourts.OracleDataApi.Client.V1.EmailHistory body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> InsertEmailHistoryAsync(TrafficCourts.OracleDataApi.Client.V1.EmailHistory body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> InsertFileHistoryAsync(TrafficCourts.OracleDataApi.Client.V1.FileHistory body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> InsertFileHistoryAsync(TrafficCourts.OracleDataApi.Client.V1.FileHistory body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> RejectDisputeAsync(long id, string body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> RejectDisputeAsync(long id, string body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string remark)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string remark, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> ResetDisputeEmailAsync(long id, string email)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> ResetDisputeEmailAsync(long id, string email, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> SaveDisputeAsync(TrafficCourts.OracleDataApi.Client.V1.Dispute body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> SaveDisputeAsync(TrafficCourts.OracleDataApi.Client.V1.Dispute body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> SaveDisputeUpdateRequestAsync(string guid, TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<long> SaveDisputeUpdateRequestAsync(string guid, TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> SubmitDisputeAsync(long id)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> SubmitDisputeAsync(long id, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> UpdateDisputeAsync(long id, TrafficCourts.OracleDataApi.Client.V1.Dispute body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> UpdateDisputeAsync(long id, TrafficCourts.OracleDataApi.Client.V1.Dispute body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequestStatus disputeUpdateRequestStatus)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequestStatus disputeUpdateRequestStatus, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.OracleDataApi.Client.V1.JJDispute body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.OracleDataApi.Client.V1.JJDispute body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.OracleDataApi.Client.V1.JJDispute body)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, TrafficCourts.OracleDataApi.Client.V1.JJDispute body, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> ValidateDisputeAsync(long id)
    {
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task<TrafficCourts.OracleDataApi.Client.V1.Dispute> ValidateDisputeAsync(long id, System.Threading.CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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
        using var operation = _metrics.BeginOperation();

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

    public async global::System.Threading.Tasks.Task DeleteViolationTicketCountAsync(long id)
    {
        using var operation = _metrics.BeginOperation();

        try
        {
            await _inner.DeleteViolationTicketCountAsync(id).ConfigureAwait(false);
            return;
        }
        catch (global::System.Exception exception)
        {
            operation.Error(exception);
            throw;
        }
    }

    public async global::System.Threading.Tasks.Task DeleteViolationTicketCountAsync(long id, CancellationToken cancellationToken)
    {
        using var operation = _metrics.BeginOperation();

        try
        {
            await _inner.DeleteViolationTicketCountAsync(id, cancellationToken).ConfigureAwait(false);
            return;
        }
        catch (global::System.Exception exception)
        {
            operation.Error(exception);
            throw;
        }
    }
}
