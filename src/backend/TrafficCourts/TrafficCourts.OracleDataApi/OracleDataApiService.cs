using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficCourts.Domain.Events;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using Oracle = TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.OracleDataApi;

internal partial class OracleDataApiService : IOracleDataApiService
{
    private const string NoticeOfDisputeGuidFormat = "d";

    private readonly Oracle.IOracleDataApiClient _client;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<OracleDataApiService> _logger;

    public OracleDataApiService(
        Oracle.IOracleDataApiClient client, 
        IMapper mapper, 
        IMediator mediator,
        ILogger<OracleDataApiService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region IOracleDataApiClient methods

    public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.AcceptJJDisputeAsync(ticketNumber, checkVTCAssigned, partId, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original

        }
    }

    public async Task AssignJJDisputesToJJAsync(IEnumerable<string> ticketNumbers, string? jjUsername, CancellationToken cancellationToken)
    {
        try
        {
            await _client.AssignJJDisputesToJJAsync(ticketNumbers, jjUsername, cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }

        await PublishEventAsync(new DisputesAssignedEvent(ticketNumbers, jjUsername), cancellationToken);
    }

    public async Task<Dispute> CancelDisputeAsync(long id, string body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.CancelDisputeAsync(id, body, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.CancelJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task CodeTableRefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            // wish the C# code could just fetch the lookups instead of tight coupling of the caching
            // of the lookup between oracle data api and C# services
            await _client.CodeTableRefreshAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.ConcludeJJDisputeAsync(ticketNumber, checkVTCAssigned, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.ConfirmJJDisputeAsync(ticketNumber, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task DeleteDisputeAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _client.DeleteDisputeAsync(id, cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }

        await PublishDisputeChanged(id, cancellationToken);
    }

    public async Task DeleteJJDisputeAsync(long? jjDisputeId, string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            await _client.DeleteJJDisputeAsync(jjDisputeId, ticketNumber, cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string? issuedTime, string? noticeOfDisputeGuid, ExcludeStatus2? excludeStatus, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.ExcludeStatus2 oracleExcludeStatus = _mapper.Map<Oracle.ExcludeStatus2>(excludeStatus);

            ICollection<Oracle.DisputeResult> oracle = await _client.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, oracleExcludeStatus, cancellationToken);

            // events?

            ICollection<DisputeResult> domain = _mapper.Map<ICollection<DisputeResult>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<DisputeListItem>> GetAllDisputesAsync(DateTimeOffset? newerThan, ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.ExcludeStatus? oracleExcludeStatus = _mapper.Map<Oracle.ExcludeStatus?>(excludeStatus);

            ICollection<Oracle.DisputeListItem> oracle = await _client.GetAllDisputesAsync(newerThan, oracleExcludeStatus, cancellationToken);

            // events?

            ICollection<DisputeListItem> domain = _mapper.Map<ICollection<DisputeListItem>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> GetDisputeAsync(long id, bool isAssign, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.GetDisputeAsync(id, isAssign, cancellationToken);

            // events?

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken);

            // events?

            // convert outputs from Oracle to Domain
            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, Status? status, CancellationToken cancellationToken)
    {
        try
        {
            // convert inputs from Domain to Oracle
            Oracle.Status domainStatus = _mapper.Map<Oracle.Status>(status);

            ICollection<Oracle.DisputeUpdateRequest> oracle = await _client.GetDisputeUpdateRequestsAsync(id, domainStatus, cancellationToken);

            // events?

            // convert outputs from Oracle to Domain
            ICollection<DisputeUpdateRequest> domain = _mapper.Map<ICollection<DisputeUpdateRequest>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<Oracle.EmailHistory> oracle = await _client.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken);

            // events?

            ICollection<EmailHistory> domain = _mapper.Map<ICollection<EmailHistory>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<Oracle.FileHistory> oracle = await _client.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);

            // events?

            ICollection<FileHistory> domain = _mapper.Map<ICollection<FileHistory>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken);

            // events?

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<ICollection<JJDispute>> GetJJDisputesAsync(string? jjAssignedTo, string? ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<Oracle.JJDispute> oracle = await _client.GetJJDisputesAsync(jjAssignedTo, ticketNumber, cancellationToken);

            // events?

            ICollection<JJDispute> domain = _mapper.Map<ICollection<JJDispute>>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.DocumentType oracleDocumentType = _mapper.Map<Oracle.DocumentType>(documentType);

            Oracle.TicketImageDataJustinDocument oracle = await _client.GetTicketImageDataAsync(ticketNumber, oracleDocumentType, cancellationToken);

            // events?

            TicketImageDataJustinDocument domain = _mapper.Map<TicketImageDataJustinDocument>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<long> InsertEmailHistoryAsync(EmailHistory body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.EmailHistory oracle = _mapper.Map<Oracle.EmailHistory>(body);

            long id = await _client.InsertEmailHistoryAsync(oracle, cancellationToken);

            // ??
            //await PublishDisputeChanged(oracle.OccamDisputeId, cancellationToken);

            return id;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<long> InsertFileHistoryAsync(FileHistory body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.FileHistory oracle = _mapper.Map<Oracle.FileHistory>(body);

            long id = await _client.InsertFileHistoryAsync(oracle, cancellationToken);

            await PublishDisputeChanged(oracle.DisputeId, cancellationToken);

            return id;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> RejectDisputeAsync(long id, string body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.RejectDisputeAsync(id, body, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> ResetDisputeEmailAsync(long id, string email, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.ResetDisputeEmailAsync(id, email, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracle = await _client.ReviewJJDisputeAsync(ticketNumber, checkVTCAssigned, recalled, body, cancellationToken);

            await PublishDisputeChanged(oracle, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<long> SaveDisputeAsync(Dispute body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracleBody = _mapper.Map<Oracle.Dispute>(body);

            // stub out the ViolationTicket if the submitted Dispute has associated OCR scan results.
            if (!string.IsNullOrEmpty(oracleBody.OcrTicketFilename))
            {
                oracleBody.ViolationTicket = CreateViolationTicketFromDispute(oracleBody);
            }

            long id = await _client.SaveDisputeAsync(oracleBody, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            return id;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.DisputeUpdateRequest oracleBody = _mapper.Map<Oracle.DisputeUpdateRequest>(body);

            long id = await _client.SaveDisputeUpdateRequestAsync(guid, oracleBody, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            return id;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> SubmitDisputeAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.SubmitDisputeAsync(id, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task UnassignDisputesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _client.UnassignDisputesAsync(cancellationToken);

            await PublishEventAsync(new DisputesUnassignedEvent(), cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> UpdateDisputeAsync(long id, Dispute body, bool checkUserAssigned, CancellationToken cancellationToken)
    {
        try
        {
            // TCVP-2977: If the dispute has an IcbcName, update the Violation Ticket's DisputantSurname and DisputantGivenNames
            if (body.IcbcName is { Surname: not null, FirstGivenName: not null })
            {
                body.ViolationTicket.DisputantSurname = body.IcbcName.Surname;
                body.ViolationTicket.DisputantGivenNames = body.IcbcName.FirstGivenName;
            
                if (body.IcbcName.SecondGivenName is not null)
                {
                    body.ViolationTicket.DisputantGivenNames = $"{body.ViolationTicket.DisputantGivenNames} {body.IcbcName.SecondGivenName}";
                }
            }
            Oracle.Dispute oracleBody = _mapper.Map<Oracle.Dispute>(body);

            Oracle.Dispute oracle = await _client.UpdateDisputeAsync(id, checkUserAssigned, oracleBody, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.DisputeUpdateRequestStatus oracleStatus = _mapper.Map<Oracle.DisputeUpdateRequestStatus>(disputeUpdateRequestStatus);

            Oracle.DisputeUpdateRequest oracle = await _client.UpdateDisputeUpdateRequestStatusAsync(id, oracleStatus, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            DisputeUpdateRequest domain = _mapper.Map<DisputeUpdateRequest>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracleBody = _mapper.Map<Oracle.JJDispute>(body);

            Oracle.JJDispute oracle = await _client.UpdateJJDisputeAsync(ticketNumber, checkVTCAssigned, oracleBody, cancellationToken);

            await PublishDisputeChanged(body, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.JJDispute oracleBody = _mapper.Map<Oracle.JJDispute>(body);

            Oracle.JJDispute oracle = await _client.UpdateJJDisputeCascadeAsync(ticketNumber, checkVTCAssigned, oracleBody, cancellationToken);

            await PublishDisputeChanged(oracleBody, cancellationToken);

            JJDispute domain = _mapper.Map<JJDispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<Dispute> ValidateDisputeAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            Oracle.Dispute oracle = await _client.ValidateDisputeAsync(id, cancellationToken);

            await PublishDisputeChanged(id, cancellationToken);

            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task VerifyDisputeEmailAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _client.VerifyDisputeEmailAsync(id, cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    private static Oracle.ViolationTicket CreateViolationTicketFromDispute(Oracle.Dispute dispute)
    {
        Oracle.ViolationTicket violationTicket = new();
        violationTicket.TicketNumber = dispute.TicketNumber;
        // Stub out the violationsTicketCounts with default count no for mapping dispute counts properly in Oracle API
        List<Oracle.ViolationTicketCount> violationTicketCounts = new();
        for (int i = 1; i <= 3; i++)
        {
            violationTicketCounts.Add(new Oracle.ViolationTicketCount { CountNo = i });
        }
        violationTicket.ViolationTicketCounts = violationTicketCounts;

        return violationTicket;
    }

    #endregion

    #region  Non Oracle Data API signatures

    public async Task<Dispute> GetDisputeByIdAsync(long disputeId, bool isAssign, CancellationToken cancellationToken)
    {
        var response = await GetDisputeAsync(disputeId, isAssign, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<Dispute?> GetDisputeByNoticeOfDisputeGuidAsync(Guid noticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        string id = noticeOfDisputeGuid.ToString(NoticeOfDisputeGuidFormat);

        try
        {
            Oracle.Dispute? oracle = await _client.GetDisputeByNoticeOfDisputeGuidAsync(id, cancellationToken);
            
            Dispute domain = _mapper.Map<Dispute>(oracle);

            return domain;
        }
        catch (Oracle.ApiException e) when (e.StatusCode == 404)
        {
            return null; // 404 is ok
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }

    public async Task<IList<DisputeResult>> SearchDisputeAsync(string? ticketNumber, string? issuedTime, Guid? noticeOfDisputeGuid, ExcludeStatus2? excludeStatus, CancellationToken cancellationToken)
    {
        // need to search by ticket number and issue time, or noticeOfDisputeGuid

        if (ticketNumber is null && issuedTime is null && noticeOfDisputeGuid is null && excludeStatus is null)
        {
            return Array.Empty<DisputeResult>(); // no values passed for searching
        }

        var noticeOfDisputeId = noticeOfDisputeGuid?.ToString(NoticeOfDisputeGuidFormat);
        Oracle.ExcludeStatus2 exclude = _mapper.Map<Oracle.ExcludeStatus2>(excludeStatus);

        try
        {
            ICollection<Oracle.DisputeResult> oracle = await _client.FindDisputeStatusesAsync(ticketNumber, issuedTime, noticeOfDisputeId, exclude, cancellationToken).ConfigureAwait(false);

            ICollection<DisputeResult> domain = _mapper.Map<ICollection<DisputeResult>>(oracle);

            return new List<DisputeResult>(domain);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }


    public async Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        var response = await InsertFileHistoryAsync(fileHistory, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken)
    {
        var response = await InsertEmailHistoryAsync(emailHistory, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task DeleteViolationTicketCountAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _client.DeleteViolationTicketCountAsync(id, cancellationToken);
        }
        catch (Exception exception)
        {
            var ex = ToOracleDataApiServiceException(exception);
            if (ex != exception)
            {
                throw ex;
            }
            throw; // throw the original
        }
    }
    #endregion

    /// <summary>
    /// Creates the Oracle Data Api Service exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns>The exception that should be thrown.</returns>
    private Exception ToOracleDataApiServiceException(Exception exception)
    {
        LogExceptionThrown(exception);

        // Oracle.ApiException is internal, so convert to core domain exception
        if (exception is Oracle.ApiException apiException)
        {
            return new TrafficCourts.Exceptions.ApiException(apiException.Message, apiException.StatusCode, apiException.Response, apiException.Headers, apiException.InnerException);
        }

        return exception;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, EventName = "ExceptionThrown", Message = "An exception was thrown")]
    private partial void LogExceptionThrown(Exception exception);

    private async Task PublishDisputeChanged(long id, CancellationToken cancellationToken)
    {
        await PublishEventAsync(new DisputeChangedEvent(id), cancellationToken);
    }

    private async Task PublishDisputeChanged(Oracle.JJDispute dispute, CancellationToken cancellationToken)
    {
        await PublishEventAsync(new DisputeChangedEvent(dispute.Id), cancellationToken);
    }

    private async Task PublishDisputeChanged(JJDispute dispute, CancellationToken cancellationToken)
    {
        await PublishEventAsync(new DisputeChangedEvent(dispute.Id), cancellationToken);
    }

    private async Task PublishEventAsync<TNotification>(TNotification notification, CancellationToken cancellationToken) where TNotification : INotification
    {
        await _mediator.Publish(notification, cancellationToken)
            .ConfigureAwait(false);
    }
}
