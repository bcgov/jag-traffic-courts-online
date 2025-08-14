using MediatR;

namespace TrafficCourts.Staff.Service.Features.CourtFiles.Summaries;

public class Request : IRequest<Response>
{
    public bool? appearances { get; set; }
    public bool? notice_of_hearing_yn { get; set; }
    public bool? multiple_officers_yn { get; set; }
    public bool? electronic_ticket_yn { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TimeZoneInfo time_zone { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string? submitted_from { get; set; }
    public string? submitted_thru { get; set; }

    public string? ticket_number { get; set; }
    public string? surname { get; set; }
    public string? surname_or_org_name { get; set; }

    public string? jj_assigned_to { get; set; }

    public string? jj_decision_dt_from { get; set; }
    public string? jj_decision_dt_thru { get; set; }

    public string? dispute_status_codes { get; set; }
    public string? appearance_courthouse_ids { get; set; }
    public string? appearance_dt_from { get; set; }
    public string? appearance_dt_thru { get; set; }

    public string? to_be_heard_at_courthouse_ids { get; set; }
    public string? hearing_type_cd { get; set; }

    public int? page_number { get; set; }
    public int? page_size { get; set; }

    public string? sort_by { get; set; }
}
