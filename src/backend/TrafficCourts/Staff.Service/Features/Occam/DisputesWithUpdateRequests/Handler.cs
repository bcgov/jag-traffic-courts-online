using MediatR;
using System.Globalization;
using System.Text;
using TrafficCourts.Domain.Models;
using TrafficCourts.OrdsDataService.Occam;

namespace TrafficCourts.Staff.Service.Features.Occam.DisputesWithUpdateRequests;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IOccamDisputeWithUpdateRequestRepository _repository;
    private readonly Serilog.ILogger _logger;

    public Handler(IOccamDisputeWithUpdateRequestRepository repository, Serilog.ILogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        try
        {
            // todo: add validation on the request
            var parameters = GetParameters(request);

            var pagedCollection = await _repository.GetListAsync(parameters, cancellationToken);

            var response = CreateResponse(pagedCollection);

            return response;
        }
        catch (Exception exception)
        {
            // generate an error id that we log and return to the client
            string errorId = Guid.NewGuid().ToString("n");

            _logger
                .ForContext("ErrorId", errorId)
                .Error(exception, "Error fetching data from ORDS");

            return new Response(errorId);
        }
    }


    private Response CreateResponse(OrdsDataService.OrdsDataServicePagedCollectionResponse<OccamDisputeWithUpdateRequest> pagedCollection)
    {
        if (pagedCollection.Rows is not null)
        {
            var items = pagedCollection.Rows.Select(Map);

            var offset = pagedCollection.Offset;
            var pageSize = pagedCollection.Fetch;
            var totalRows = pagedCollection.TotalRows;

            int pageNumber = (offset / pageSize) + 1;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            var pagedList = new PagedOccamDisputeWithUpdateRequestListItemCollection(items, pageNumber, pageSize, pagedCollection.TotalRows);
            return new Response(pagedList);
        }

        // generate an error id that we log and return to the client
        string errorId = Guid.NewGuid().ToString("n");

        var error = pagedCollection.Errors?.FirstOrDefault();
        var logger = _logger.ForContext("ErrorId", errorId);

        if (error is not null)
        {
            logger
                .ForContext("ErrorCode", error.ErrorCode)
                .ForContext("ErrorMessage", error.ErrorMessage)
                .ForContext("ErrorStack", error.ErrorStack)
                .Error("Error fetching data from ORDS");
        }
        else
        {
            logger
                .Warning("No data returned from ORDS, no error details are available");
        }

        return new Response(errorId);
    }

    private Dictionary<string, string> GetParameters(Request request)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        AdjustRequest(request);

        AddInclude(parameters, request);
        AddWhere(parameters, request);
        AddOrderBy(parameters, request);
        AddPaging(parameters, request);

        return parameters;
    }

    private void AdjustRequest(Request request)
    {
    }

    private void AddInclude(Dictionary<string, string> parameters, Request request)
    {
    }

    private void AddWhere(Dictionary<string, string> parameters, Request request)
    {
        AddUtcDateRangeFilter(parameters, "submitted_dt", request.Parameters.TimeZone, request.Parameters.From, request.Parameters.Thru);
        AddStartFilter(parameters, "ticket_number_txt", request.Parameters.TicketNumber);
        AddStartFilter(parameters, "upper_disputant_surname_nm", request.Parameters.Surname?.ToUpper(), false);

        // We're hardcoding this to always filter on PEND because that's what V1 API did.
        // Probably this should come from the UI instead?
        // At the very least, it should come from an ENUM, but we don't have an enum for Request Statuses in C# yet
        AddEqualityFilter(parameters, "dispute_update_stat_type_cd", "PEND");

        if (request.Parameters.Status is not null)
        {
            var statusCodes = request.Parameters.Status.Select(s => ToDisputeStatusCode(s));
            parameters.Add("dispute_status_type_cd_in", string.Join(',', statusCodes)); 
        }

        if (request.Parameters.CourtHouseIds is not null)
        {
            var courthouseIds = request.Parameters.CourtHouseIds;
            parameters.Add("court_agen_id_in", string.Join(',', courthouseIds));
        }
    }

    private void AddEqualityFilter(Dictionary<string, string> parameters, string field, string? value, bool toUpper = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        value = value.Trim(); // remove wrapping spaces

        if (toUpper)
        {
            value = value.ToUpper();
        }

        parameters.Add($"{field}_eq", $"{value}");
    }

    private void AddStartFilter(Dictionary<string, string> parameters, string field, string? value, bool toUpper = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        value = value.Trim(); // remove wrapping spaces

        if (toUpper)
        {
            value = value.ToUpper();
        }

        parameters.Add($"{field}_like", $"{value}%");
    }

    private void AddUtcDateRangeFilter(Dictionary<string, string> parameters, string field, string? timeZone, string? from, string? thru)
    {
        if (from is null && thru is null)
        {
            return;
        }

        timeZone = timeZone ?? "America/Vancouver";
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

        // convert the from/thru to UTC
        if (from is not null)
        {
            DateTime date = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = TimeZoneInfo.ConvertTimeToUtc(date, tz);
            parameters.Add($"{field}_ge", date.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        if (thru is not null)
        {
            // bump the date by one and search for less than the next day
            DateTime date = DateTime.ParseExact(thru, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = date.AddDays(1);
            date = TimeZoneInfo.ConvertTimeToUtc(date, tz);
            parameters.Add($"{field}_lt", date.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }

    private void AddOrderBy(Dictionary<string, string> parameters, Request request)
    {
        if (request.Parameters.SortBy is null)
        {
            return;
        }

        var buffer = new StringBuilder();
        var clientOrder = request.Parameters.SortBy.ToArray();
        var clientDirection = request.Parameters.SortDirection.ToArray();

        void Append(Collections.SortDirection direction, string target)
        {
            if (buffer.Length > 0)
            {
                buffer.Append(',');
            }
            if (direction == Collections.SortDirection.desc)
            {
                buffer.Append("-");
            }
            buffer.Append(target);
        }

        for (int i = 0; i < clientOrder.Length; i++)
        {
            var item = clientOrder[i];
            var direction = clientDirection[i];

            // map the model to the column name
            string? target = item switch
            {
                "disputeId"                             => "dispute_id",
                "submittedTs"                           => "submitted_dt",
                "ticketNumber"                          => "ticket_number_txt",
                "disputantSurname"                      => "upper_disputant_surname_nm",
                "disputantGivenName1"                   => "upper_disputant_given_1_nm",
                "status"                                => "dispute_status_type_cd",
                "requestCourtAppearanceYn"              => "request_court_appearance_yn",
                "disputantDetectedOcrIssues"            => "disputant_detect_ocr_issues_yn",
                "interpreterRequired"                   => "interpreter_required_yn",
                "userAssignedTo"                        => "user_assigned_to",
                "courthouseLocation"                    => "court_agen_nm",
                "hearingDate"                           => "hearing_dt",
                "updateRequest_OldestDate"              => "update_request_submitted_dt",
                "updateRequest_HasChangeOfPlea"         => "update_request_change_of_plea_yn",
                "updateRequest_HasAdjournmentDocument"  => "update_request_adjournment_document_yn",
                _                                       => null
            };

            if (target is not null)
            {
                Append(direction, target);
            }
            else
            {
                _logger.Warning("Unknown sort column {Column}", item);
            }
        }

        parameters.Add("order", buffer.ToString());
    }

    private void AddPaging(Dictionary<string, string> parameters, Request request)
    {
        if (request.Parameters.PageSize is not null)
        {
            parameters.Add("fetch_rows", request.Parameters.PageSize.Value.ToString());

            if (request.Parameters.PageSize.Value == -1)
            {
                // caller wants all the rows
                parameters.Add("offset_rows", "0");
                return;
            }
        }

        int pageSize = request.Parameters.PageSize ?? 25;

        if (request.Parameters.PageNumber is not null)
        {

            // compute the offset_rows
            int offset = (request.Parameters.PageNumber.Value - 1) * pageSize;
            parameters.Add("offset_rows", offset.ToString());
        }
    }

    private static YesNo? ToYesNo(string? value)
    {
        return value switch
        {
            "Y" => YesNo.Yes,
            "N" => YesNo.No,
            null => null,
            _ => YesNo.Unknown
        };
    }

    private static DisputeListItemStatus ToDisputeStatusEnum(string value)
    {
        return value switch
        {
            "NEW"  => DisputeListItemStatus.NEW,
            "VALD" => DisputeListItemStatus.VALIDATED,
            "PROC" => DisputeListItemStatus.PROCESSING,
            "REJ"  => DisputeListItemStatus.REJECTED,
            "CANC" => DisputeListItemStatus.CANCELLED,
            "CNLD" => DisputeListItemStatus.CONCLUDED,
            _      => DisputeListItemStatus.UNKNOWN,
        };
    }

    private static string ToDisputeStatusCode(DisputeStatus value)
    {
        return value switch
        {
            DisputeStatus.NEW           => "NEW",
            DisputeStatus.VALIDATED     => "VALD",
            DisputeStatus.PROCESSING    => "PROC",
            DisputeStatus.REJECTED      => "REJ",
            DisputeStatus.CANCELLED     => "CANC",
            DisputeStatus.CONCLUDED     => "CNLD",
            DisputeStatus.UNKNOWN       => null,
            _                           => null,
        };
    }

    private OccamDisputeWithUpdateRequestListItemModel Map(OccamDisputeWithUpdateRequest dispute)
    {
        // all of the == "Y" stuff is a temporary hack to maintain V1 support in the front-end - should probably all be YesNo properties instead of unique enums
        var listItem = new OccamDisputeWithUpdateRequestListItemModel
        {
            disputeId = dispute.dispute_id,
            ticketNumber = dispute.ticket_number_txt,
            submittedTs = dispute.submitted_dt,
            disputantSurname = dispute.disputant_surname_nm,
            disputantGivenName1 = dispute.disputant_given_1_nm,
            disputantGivenName2 = dispute.disputant_given_2_nm,
            disputantGivenName3 = dispute.disputant_given_3_nm,
            status = ToDisputeStatusEnum(dispute.dispute_status_type_cd),
            emailAddress = dispute.email_address_txt,
            emailAddressVerified = dispute.email_verified_yn == "Y", 
            filingDate = dispute.filing_dt,
            requestCourtAppearanceYn = dispute.request_court_appearance_yn == "Y" ? DisputeRequestCourtAppearanceYn.Y : DisputeRequestCourtAppearanceYn.N,
            userAssignedTo = dispute.user_assigned_to,
            disputantDetectedOcrIssues = dispute.disputant_detect_ocr_issues_yn == "Y" ? DisputeDisputantDetectedOcrIssues.Y : DisputeDisputantDetectedOcrIssues.N,
            systemDetectedOcrIssues = dispute.system_detect_ocr_issues_yn == "Y" ? DisputeSystemDetectedOcrIssues.Y : DisputeSystemDetectedOcrIssues.N,
            interpreterRequired = dispute.interpreter_required_yn == "Y" ? DisputeInterpreterRequired.Y : DisputeInterpreterRequired.N,
            violationDate = dispute.violation_dt,
            jjAssignedTo = dispute.jj_assigned_to,
            decisionMadeBy = dispute.most_recent_decision_made_by,
            jjDecisionDate = dispute.jj_decision_dt,
            courtAgenId = dispute.court_agen_id.ToString(),
            courtAgenName = dispute.court_agen_nm,
            hearingDate = dispute.hearing_dt,
            updateRequest_OldestDate = dispute.update_request_submitted_dt,
            updateRequest_HasChangeOfPlea = dispute.update_request_change_of_plea_yn,
            updateRequest_HasAdjournmentDocument = dispute.update_request_adjournment_document_yn,
        };

        return listItem;
    }
}
