using MediatR;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Request : IRequest<Response>
{
    public OccamDisputeListingParameters Parameters { get; set; }

    //public string? time_zone { get; set; }
    //public string? submitted_from { get; set; }
    //public string? submitted_thru { get; set; }

    //public string? ticket_number { get; set; }
    //public string? surname { get; set; }
    //public string? dispute_status_codes { get; set; }
    //public string? appearance_courthouse_ids { get; set; }

    //public int? page_number { get; set; }
    //public int? page_size { get; set; }

    //public string? sort_by { get; set; }
}
