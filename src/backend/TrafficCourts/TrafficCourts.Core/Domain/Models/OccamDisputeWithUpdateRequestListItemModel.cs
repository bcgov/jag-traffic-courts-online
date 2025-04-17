namespace TrafficCourts.Domain.Models;

/// <summary>
/// Represents a subset of a dispute case file, used for displaying lists of dispute case files
/// </summary>

public class PagedOccamDisputeWithUpdateRequestListModel
{
    public OccamDisputeWithUpdateRequestListItemModel[] items { get; set; }
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public int pageCount { get; set; }
    public int totalItemCount { get; set; }
    public bool hasPreviousPage { get; set; }
    public bool hasNextPage { get; set; }
    public bool isFirstPage { get; set; }
    public bool isLastPage { get; set; }
}

public class OccamDisputeWithUpdateRequestListItemModel
{
    public int disputeId { get; set; }
    public string ticketNumber { get; set; }
    public DateTime submittedTs { get; set; }
    public string disputantSurname { get; set; }
    public string disputantGivenName1 { get; set; }
    public string disputantGivenName2 { get; set; }
    public string disputantGivenName3 { get; set; }
    public DisputeListItemStatus status { get; set; }
    public string emailAddress { get; set; }
    // This should be a YesNo, but the current Front End expects a boolean for some reason
    // The front end isn't being changed at this time to aavoid breaking backwards compatibility of the dispute service for pages still using the v1 endpoints
    public bool? emailAddressVerified { get; set; }
    public DateTime? filingDate { get; set; }
    public DisputeRequestCourtAppearanceYn requestCourtAppearanceYn { get; set; }
    public string userAssignedTo { get; set; }
    public DisputeDisputantDetectedOcrIssues disputantDetectedOcrIssues { get; set; }
    public DisputeSystemDetectedOcrIssues systemDetectedOcrIssues { get; set; }
    public DisputeInterpreterRequired interpreterRequired { get; set; }
    public DateTime? violationDate { get; set; }
    public string jjAssignedTo { get; set; }
    public string decisionMadeBy { get; set; }
    public DateTime? jjDecisionDate { get; set; }
    public string courtAgenId { get; set; }
    public string courtAgenName { get; set; }
    public DateTime? hearingDate { get; set; }
    public DateTime? updateRequest_OldestDate { get; set; }
    public string updateRequest_HasChangeOfPlea { get; set; }
    public string updateRequest_HasAdjournmentDocument { get; set; }
}

public class Rootobject
{
    public int dispute_update_request_id { get; set; }
    public int dispute_id { get; set; }
    public string dispute_update_stat_type_cd { get; set; }
    public string dispute_update_req_type_cd { get; set; }
    public string request_json_txt { get; set; }
    public string current_json_txt { get; set; }
    public DateTime status_update_dtm { get; set; }
    public DateTime ent_dtm { get; set; }
    public string ent_user_id { get; set; }
    public DateTime upd_dtm { get; set; }
    public string upd_user_id { get; set; }
    public DateTime submitted_dt { get; set; }
    public string disputant_surname_nm { get; set; }
    public string disputant_given_1_nm { get; set; }
    public object disputant_given_2_nm { get; set; }
    public object disputant_given_3_nm { get; set; }
    public string dispute_status_type_cd { get; set; }
    public string notice_of_dispute_guid { get; set; }
    public object court_agen_id { get; set; }
    public string ticket_number_txt { get; set; }
    public int violation_ticket_upload_id { get; set; }
}

