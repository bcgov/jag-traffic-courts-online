namespace TrafficCourts.Domain.Models;

/// <summary>
/// Represents a subset of a dispute case file, used for displaying lists of dispute case files
/// </summary>

public class OccamPagedListModel
{
    public OccamDisputeListItemModel[] items { get; set; }
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public int pageCount { get; set; }
    public int totalItemCount { get; set; }
    public bool hasPreviousPage { get; set; }
    public bool hasNextPage { get; set; }
    public bool isFirstPage { get; set; }
    public bool isLastPage { get; set; }
}

public class OccamDisputeListItemModel
{
    public int disputeId { get; set; }
    public string ticketNumber { get; set; }
    public DateTime submittedTs { get; set; }
    public string disputantSurname { get; set; }
    public string disputantGivenName1 { get; set; }
    public string disputantGivenName2 { get; set; }
    public string disputantGivenName3 { get; set; }
    public string status { get; set; }
    public string emailAddress { get; set; }
    public YesNo? emailAddressVerified { get; set; }
    public DateTime? filingDate { get; set; }
    public YesNo? requestCourtAppearanceYn { get; set; }
    public string userAssignedTo { get; set; }
    public YesNo? disputantDetectedOcrIssues { get; set; }
    public YesNo? systemDetectedOcrIssues { get; set; }
    public YesNo? interpreterRequired { get; set; }
    public DateTime? violationDate { get; set; }
    public string jjAssignedTo { get; set; }
    public string decisionMadeBy { get; set; }
    public DateTime? jjDecisionDate { get; set; }
    public float? courtAgenId { get; set; }
}
