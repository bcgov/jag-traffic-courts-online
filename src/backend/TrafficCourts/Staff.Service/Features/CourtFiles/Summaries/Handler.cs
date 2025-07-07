using MediatR;
using System.Globalization;
using System.Text;
using TrafficCourts.Domain.Models;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.Staff.Service.Features.CourtFiles.Summaries;


public class Handler : IRequestHandler<Request, Response>
{
    private readonly IDisputeCaseFileSummaryRepository _repository;
    private readonly Serilog.ILogger _logger;

    public Handler(IDisputeCaseFileSummaryRepository repository, Serilog.ILogger logger)
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

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.time_zone ?? "America/Vancouver");
            var response = CreateResponse(pagedCollection, timeZone);

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

    private Response CreateResponse(OrdsDataService.OrdsDataServicePagedCollectionResponse<OrdsDisputeCaseFileSummary> pagedCollection, TimeZoneInfo timeZone)
    {
        if (pagedCollection.Rows is not null)
        {
            var items = pagedCollection.Rows.Select(_ => Map(_, timeZone));

            var offset = pagedCollection.Offset;
            var pageSize = pagedCollection.Fetch;
            var totalRows = pagedCollection.TotalRows;

            int pageNumber = (offset / pageSize) + 1;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            var pagedList = new PagedDisputeCaseFileSummaryCollection(items, pageNumber, pageSize, pagedCollection.TotalRows);
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
        // ensure appearances are fetched if filtering or sorting on the column, otherwise we'd have to 
        // return bad request
        if (request.appearance_courthouse_ids is not null)
        {
            request.appearances = true;
        }

        if (request.sort_by is not null)
        {
            if (request.sort_by.Contains("appearanceCourthouseName") ||
                request.sort_by.Contains("appearanceRoomCode") ||
                request.sort_by.Contains("appearanceTs"))
            {
                request.appearances = true;
            }
        }

        // notice_of_hearing_yn
        // multiple_officers_yn
        // electronic_ticket_yn
    }

    private void AddInclude(Dictionary<string, string> parameters, Request request)
    {
        HashSet<string> include = new HashSet<string>();

        if (request.appearances is true) include.Add("appearances");
        if (request.notice_of_hearing_yn is true) include.Add("notice_of_hearing_yn");
        if (request.multiple_officers_yn is true) include.Add("multiple_officers_yn");
        if (request.electronic_ticket_yn is true) include.Add("electronic_ticket_yn");
        if (include.Count > 0) parameters.Add("include", string.Join(",", include));
    }

    private void AddWhere(Dictionary<string, string> parameters, Request request)
    {
        AddUtcDateRangeFilter(parameters, "submitted_dt", request.time_zone, request.submitted_from, request.submitted_thru);
        AddUtcDateRangeFilter(parameters, "jj_decision_dt", request.time_zone, request.jj_decision_dt_from, request.jj_decision_dt_thru);

        AddDateRangeFilter(parameters, "appr_tm", request.appearance_dt_from, request.appearance_dt_thru);
        
        // Search where starts with the supplied value. We could also use "_regexp_like" and use a regex pattern
        // ticket number is always uppercase
        AddLikeFilter(parameters, "ticket_number_txt", request.ticket_number);
        AddLikeFilter(parameters, "prof_surname_nm", request.surname);
        AddLikeFilter(parameters, "prof_surname_nm_or_org_nm", request.surname_or_org_name);

        if (request.jj_assigned_to is not null) parameters.Add("jj_assigned_to_eq", request.jj_assigned_to);

        if (request.dispute_status_codes is not null) parameters.Add("dispute_status_type_cd_in", request.dispute_status_codes);
        if (request.to_be_heard_at_courthouse_ids is not null) parameters.Add("to_be_heard_at_agen_id_in", request.to_be_heard_at_courthouse_ids);
        if (request.hearing_type_cd is not null) parameters.Add("hearing_type_cd_eq", request.hearing_type_cd);

        if (request.appearance_courthouse_ids is not null && request.appearances is true)
        {
            parameters.Add("appr_ctrm_agen_id_in", request.appearance_courthouse_ids);
        }
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
            parameters.Add($"{field}_ge", date.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        if (thru is not null)
        {
            // bump the date by one and search for less than the next day
            DateTime date = DateTime.ParseExact(thru, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            date = date.AddDays(1);
            date = TimeZoneInfo.ConvertTimeToUtc(date, tz);
            parameters.Add($"{field}_lt", date.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }

    private void AddOrderBy(Dictionary<string, string> parameters, Request request)
    {
        if (request.sort_by is null)
        {
            return;
        }

        StringBuilder buffer = new StringBuilder();
        string[] clientOrder = request.sort_by.Split(',');

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

            // special handling for appearance duration as it is two fields in the database
            if (target is null && item == "appearanceDuration")
            {
                if (request.appearances is true)
                {
                    Append(direction, "appr_estimated_duration_hh");
                    Append(direction, "appr_estimated_duration_mi");
                }
                else
                {
                    _logger.Warning("{IncludeItem} was not included, sorting on {Column} will be ingored", "appearances", item);
                }

                continue;
            }

            // did we try to sort on columns not included?
            if (target == "appr_ctrm_agen_nm" || target == "appr_ctrm_room_cd" || target == "appr_tm")
            {
                if (request.appearances is not true)
                {
                    _logger.Warning("{IncludeItem} was not included, sorting on {Column} will be ingored", "appearances", item);
                    continue;
                }
            }

            if (target == "notice_of_hearing_yn")
            {
                if (request.notice_of_hearing_yn is not true)
                {
                    _logger.Warning("{IncludeItem} was not included, sorting on {Column} will be ingored", "notice_of_hearing_yn", item);
                    continue;
                }
            }

            if (target == "multiple_officers_yn")
            {
                if (request.multiple_officers_yn is not true)
                {
                    _logger.Warning("{IncludeItem} was not included, sorting on {Column} will be ingored", "multiple_officers_yn", item);
                    continue;
                }
            }

            if (target == "electronic_ticket_yn")
            {
                if (request.electronic_ticket_yn is not true)
                {
                    _logger.Warning("{IncludeItem} was not included, sorting on {Column} will be ingored", "electronic_ticket_yn", item);
                    continue;
                }
            }

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
        if (request.page_size is not null)
        {
            parameters.Add("fetch_rows", request.page_size.Value.ToString());

            if (request.page_size.Value == -1)
            {
                // caller wants all the rows
                parameters.Add("offset_rows", "0");
                return;
            }
        }

        int pageSize = request.page_size ?? 25;

        if (request.page_number is not null)
        {
            
            // compute the offset_rows
            int offset = (request.page_number.Value - 1) * pageSize;
            parameters.Add("offset_rows", offset.ToString());
        }
    }


    private DisputeCaseFileSummary Map(OrdsDisputeCaseFileSummary dispute, TimeZoneInfo timeZone)
    {
        var summary = new DisputeCaseFileSummary
        {
            Id = dispute.dispute_id,
            SubmittedTs = dispute.submitted_dt.UtcToLocalTime(timeZone),
            JjDecisionDate = dispute.jj_decision_dt.UtcToLocalTime(timeZone),
            SignatoryName = dispute.signed_by,
            HearingType = dispute.hearing_type_cd,
            TicketNumber = dispute.ticket_number_txt,
            ViolationDate = dispute.violation_dt,
            ViolationDateCount = dispute.unique_violation_dt_count,
            ToBeHeardAtCourthouseId = dispute.to_be_heard_at_agen_id,
            ToBeHeardAtCourthouseName = dispute.to_be_heard_at_agen_nm,
            DisputantSurname = dispute.prof_surname_nm,
            DisputantGivenName1 = dispute.prof_given_1_nm,
            DisputantGivenName2 = dispute.prof_given_2_nm,
            DisputantGivenName3 = dispute.prof_given_3_nm,
            DisputantOrganizationName = dispute.prof_org_nm,
            FineReductionReason = dispute.fine_reduction_reason_txt,
            TimeToPayReason = dispute.time_to_pay_reason_txt,
            DisputeStatus = new DisputeCaseFileStatus
            {
                Code = dispute.dispute_status_type_cd,
                Description = dispute.dispute_status_type_dsc
            },
            PoliceDetachmentId = dispute.detachment_agen_id,
            PoliceDetachment = dispute.detachment_agency_nm,
            AccidentYn = ToYesNo(dispute.accident_yn),
            NoticeOfHearingYn = ToYesNo(dispute.notice_of_hearing_yn),
            MultipleOfficersYn = ToYesNo(dispute.multiple_officers_yn),
            ElectronicTicketYn = ToYesNo(dispute.electronic_ticket_yn),
            JjAssignedTo = dispute.jj_assigned_to,
            VtcAssignedTo = dispute.vtc_assigned_to,
            VtcAssignedTs = dispute.vtc_assigned_dtm.UtcToLocalTime(timeZone),
            AppearanceCourthouseId = dispute.appr_ctrm_agen_id,
            AppearanceCourthouseName = dispute.appr_ctrm_agen_nm,
            AppearanceRoomCode = dispute.appr_ctrm_room_cd,
            AppearanceTs = dispute.appr_tm,
            AppearanceDuration = (dispute.appr_estimated_duration_hh ?? 0) * 60 + (dispute.appr_estimated_duration_mi ?? 0)
        };

        return summary;
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
}
