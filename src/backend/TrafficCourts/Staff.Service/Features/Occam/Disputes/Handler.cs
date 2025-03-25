using MediatR;
using System.Globalization;
using System.Text;
using TrafficCourts.Domain.Models;
using TrafficCourts.OrdsDataService.Occam;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IOccamDisputeRepository _repository;
    private readonly Serilog.ILogger _logger;

    public Handler(IOccamDisputeRepository repository, Serilog.ILogger logger)
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


    private Response CreateResponse(OrdsDataService.OrdsDataServicePagedCollectionResponse<OccamDispute> pagedCollection)
    {
        if (pagedCollection.Rows is not null)
        {
            var items = pagedCollection.Rows.Select(Map);

            var offset = pagedCollection.Offset;
            var pageSize = pagedCollection.Fetch;
            var totalRows = pagedCollection.TotalRows;

            int pageNumber = (offset / pageSize) + 1;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            var pagedList = new PagedOccamDisputeListItemCollection(items, pageNumber, pageSize, pagedCollection.TotalRows);
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

        // Search where starts with the supplied value. We could also use "_regexp_like" and use a regex pattern
        // ticket number is always uppercase
        AddLikeFilter(parameters, "ticket_number_txt", request.Parameters.TicketNumber);
        AddLikeFilter(parameters, "prof_surname_nm", request.Parameters.Surname);

        if (request.Parameters.Status is not null) parameters.Add("dispute_status_type_cd_in", string.Join(',', request.Parameters.Status));
    }

    private void AddLikeFilter(Dictionary<string, string> parameters, string field, string? value, bool toUpper = true)
    {
        if (value is null)
        {
            return;
        }

        if (toUpper)
        {
            value = value.ToUpper();
        }

        value = value.TrimEnd(); // remove trailing spaces
        parameters.Add($"{field}_like", $"{value}%");
    }

    private void AddDateRangeFilter(Dictionary<string, string> parameters, string field, string? from, string? thru)
    {
        if (from is null && thru is null)
        {
            return;
        }

        if (from is not null)
        {
            DateTime date = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            parameters.Add($"{field}_ge", date.ToString("yyyy-MM-dd"));
        }

        if (thru is not null)
        {
            // bump the date by one and search for less than the next day
            DateTime date = DateTime.ParseExact(thru, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = date.AddDays(1);
            parameters.Add($"{field}_lt", date.ToString("yyyy-MM-dd"));
        }

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

        StringBuilder buffer = new StringBuilder();
        string[] clientOrder = request.Parameters.SortBy.ToArray();

        void Append(string direction, string target)
        {
            if (buffer.Length > 0)
            {
                buffer.Append(',');
            }
            buffer.Append(direction);
            buffer.Append(target);
        }

        for (int i = 0; i < clientOrder.Length; i++)
        {
            string direction = "";
            var item = clientOrder[i];
            if (item.StartsWith('-'))
            {
                direction = "-";
                item = item.Substring(1);
            }

            // map the model to the column name
            string? target = item switch
            {
                "submittedTs" => "submitted_dt",
                "jjDecisionDate" => "jj_decision_dt",
                "signatoryName" => "signed_by",
                "hearingType" => "hearing_type_cd",
                "ticketNumber" => "ticket_number_txt",
                "violationDate" => "violation_dt",
                "toBeHeardAtCourthouseName" => "to_be_heard_at_agen_nm",
                "surname" => "prof_surname_nm",
                "surnameOrOrgName" => "prof_surname_nm_or_org_nm",
                "disputantGivenName1" => "prof_given_1_nm",
                "status" => "dispute_status_type_dsc",
                "policeDetachment" => "detachment_agency_nm",
                "accidentYn" => "accident_yn",
                "noticeOfHearingYn" => "notice_of_hearing_yn",
                "multipleOfficersYn" => "multiple_officers_yn",
                "electronicTicketYn" => "electronic_ticket_yn",
                "jjAssignedTo" => "jj_assigned_to",
                "vtcAssignedTo" => "vtc_assigned_to",
                "vtcAssignedTs" => "vtc_assigned_dtm",
                "appearanceCourthouseName" => "appr_ctrm_agen_nm",
                "appearanceRoomCode" => "appr_ctrm_room_cd",
                "appearanceTs" => "appr_tm",
                _ => null
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

    private OccamDisputeListItemModel Map(OccamDispute dispute)
    {
        var listItem = new OccamDisputeListItemModel
        {
            disputeId = dispute.dispute_id,
            ticketNumber = dispute.ticket_number_txt,
            submittedTs = dispute.submitted_dt,
            disputantSurname = dispute.disputant_surname_nm,
            disputantGivenName1 = dispute.disputant_given_1_nm,
            disputantGivenName2 = dispute.disputant_given_2_nm,
            disputantGivenName3 = dispute.disputant_given_3_nm,
            status = dispute.dispute_status_type_cd,
            emailAddress = dispute.email_address_txt,
            emailAddressVerified = ToYesNo(dispute.email_verified_yn),
            filingDate = dispute.filing_dt,
            requestCourtAppearanceYn = ToYesNo(dispute.request_court_appearance_yn),
            userAssignedTo = dispute.user_assigned_to,
            userAssignedTs = null, // TODO-DKAY - why isn't this in our model?
            disputantDetectedOcrIssues = ToYesNo(dispute.disputant_detect_ocr_issues_yn),
            systemDetectedOcrIssues = ToYesNo(dispute.system_detect_ocr_issues_yn),
            interpreterRequired = ToYesNo(dispute.interpreter_required_yn),
            violationDate = dispute.violation_dt,
            //jjDisputeStatus = dispute.dispute_status_type_cd, // ??
            jjAssignedTo = dispute.jj_assigned_to,
            decisionMadeBy = dispute.most_recent_decision_made_by,
            jjDecisionDate = dispute.jj_decision_dt,
            courtAgenId = dispute.court_agen_id,
        };

        return listItem;
    }
}
